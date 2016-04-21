using System;
using System.Collections.Generic;
using System.Text;
using Phidgets; //For the Stepper class and the exceptions class
using Phidgets.Events; //For the event handling classes
namespace Move_cable
{

    public class MyStepper
    {
        const double pas = 2.5; //Pas de la vis sans fin en mm
        const int R = 10; // Rayon de la vis sans fin en mm
        const double dt = 0.07; // Pas de temps donne par GrassHoper en sec.
        public List<double> liste;
        public int id_motor;
        public int initial_position;
        double a0 = 2 * Math.PI * Math.Sqrt(R * R + (pas / (2 * Math.PI)) * (pas / (2 * Math.PI)));
        public double derive;
        public double Goal_Position;
        public Stepper stepper;
        static bool tourne;
        public MyStepper(int id, int init_pos = 0)
        {
            id_motor = id;
            initial_position = init_pos;
            stepper = new Stepper();
            liste = new List<double>();
            tourne = true;
        }
        public void load()
        {
            try
            {
                // Read File corresponding to Motor ID
                load_trajectory();
                //Hook the basic event handlers
                stepper.Attach += new AttachEventHandler(stepper_Attach);
                stepper.Detach += new DetachEventHandler(stepper_Detach);
                stepper.Error += new ErrorEventHandler(stepper_Error);
                stepper.open();
           
                Console.WriteLine("Waiting for a Stepper to be attached....");
                stepper.waitForAttachment();
                Console.ReadLine();

                //Acceleration at Max
                stepper.steppers[0].Acceleration = stepper.steppers[0].AccelerationMax; //ensure the value is between the AccelerationMin and AccelerationMax
                Console.WriteLine("Stepper motor acceleration set to: {0}",
                                        stepper.steppers[0].Acceleration.ToString());
                stepper.steppers[0].Engaged = true;
                stepper.steppers[0].VelocityLimit = 0;
                stepper.steppers[0].CurrentLimit = 1.7;
                //stepper.steppers[0].TargetPosition = 0;
                Goal_Position = stepper.steppers[0].TargetPosition;

            }
            catch (PhidgetException ex)
            {
                Console.WriteLine(ex.Description);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public int nbr_step()
        {
            int i = 0;
            foreach(double l in liste)
            {
                i++;
            }
            return i;
        }
        public void run_step(int i)
        {
            
            double l0 = liste[i];
            double l1 = liste[i+1];
            double dx = 16 * (l0 - l1) * 200 / a0;
            double speed = Math.Abs(dx) / dt;

            stepper.steppers[0].VelocityLimit = speed;
            derive += Goal_Position - (int)Goal_Position;
            if (derive >= 1)
            {
                dx += 1;
                derive = 1 - (int)derive;
            }
            else if (derive <= -1)
            {
                dx += -1;
                derive = (int)derive + 1;
            }
            Console.WriteLine(dx);
            // Don't know why but abs(dx) must be higher than 2 to work
            Goal_Position += dx;
            if (Math.Abs(dx) < 2)
            {
                System.Threading.Thread.Sleep((int)(dt * 1000));
            }
            else
            {
                try
                {
                    stepper.steppers[0].TargetPosition = (int)Goal_Position;
                }
                catch (PhidgetException ex)
                {
                    Console.WriteLine(ex.Description);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            i++;
        }
        public bool isStopped()
        {
            return stepper.steppers[0].Stopped;
        }
        public void close()
        {
            stepper.steppers[0].Engaged = false;
            //close the Stepper object
            stepper.close();
            //set the object to null to get it out of memory
            stepper = null;
        }
        public void load_trajectory()
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\lcort_000\Desktop\coords" + id_motor.ToString()+ ".txt");
            foreach (string line in lines)
            {
                liste.Add(Convert.ToDouble(line.Replace('.', ',')));
            }


        }
        public void stepper_Attach(object sender, AttachEventArgs e)
        {
            Console.WriteLine("{0} {1} attached!", e.Device.Name,
                                        e.Device.SerialNumber.ToString());
        }

        //Detach event handler....Display the name and serial number of the detached 
        //Stepper device
        public void stepper_Detach(object sender, DetachEventArgs e)
        {
            Console.WriteLine("{0} {1} detached!", e.Device.Name,
                                        e.Device.SerialNumber.ToString());
        }

        //Error event handler....Display the description of the error
        public void stepper_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Description);
        }

        //Current Change event handler....Display the motor index and new value of 
        //the current
        public void stepper_CurrentChange(object sender, CurrentChangeEventArgs e)
        {
            Console.WriteLine("Stepper Motor {0} current changed! New Value: {1}",
                                        e.Index.ToString(), e.Current.ToString());
        }

        //Input Change event handler....Display the Input index and the new input value
        public void stepper_InputChange(object sender, InputChangeEventArgs e)
        {
            Console.WriteLine("Input {0} changed! New Value: {1}", e.Index.ToString(),
                                        e.Value.ToString());
        }

        //Position Change event handler. Display the motor index and the position value
        public void stepper_PositionChange(object sender,
                                                StepperPositionChangeEventArgs e)
        {
            Console.WriteLine("Stepper Motor {0} position: {1}", e.Index,
                                        e.Position.ToString());

        }

        //Velocity Change event handler. Display the motor index and the velocity value
        public void stepper_VelocityChange(object sender, VelocityChangeEventArgs e)
        {
            Console.WriteLine("Stepper Motor {0} velocity: {1}", e.Index,
                                        e.Velocity.ToString());
        }
        public bool isAttached()
        {
            return stepper.Attached;
        }
    }
}
