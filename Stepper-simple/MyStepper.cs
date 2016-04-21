using System;
using System.Collections.Generic;
using System.Text;
using Phidgets; //For the Stepper class and the exceptions class
using Phidgets.Events; //For the event handling classes
namespace Stepper_simple
{
    public class MyStepper
    {
        const double pas = 2.5; //Pas de la vis sans fin en mm
        const int R = 10; // Rayon de la vis sans fin en mm
        const double dt = 0.5; // Pas de temps donne par GrassHoper en sec.
        static List<double> liste = new List<double>();
        static int id_motor;
        static int initial_position;
        const double a0 = 2 * Math.PI * Math.Sqrt(R * R + (pas / (2 * Math.PI)) * (pas / (2 * Math.PI)));
        static double derive = 0;
        static Stepper stepper = new Stepper();
        public MyStepper(int id, int init_pos = 0)
        {
            id_motor = id;
            initial_position = init_pos;
        }
        static void loadStepper()
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


                //Acceleration at Max
                stepper.steppers[0].Acceleration = stepper.steppers[0].AccelerationMax; //ensure the value is between the AccelerationMin and AccelerationMax
                Console.WriteLine("Stepper motor acceleration set to: {0}",
                                        stepper.steppers[0].Acceleration.ToString());

                stepper.steppers[0].VelocityLimit = 15000;
                stepper.steppers[0].Engaged = true;
                stepper.steppers[0].TargetPosition = (int)initial_position;

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
        static void run_step()
        {
            double l0 = liste[0];
            liste.RemoveAt(0);
            double l1 = liste[0];
            double dx = 16 * (l1 - l0) * 200 / a0;
            double speed = Math.Abs(dx) / dt;
            Goal_Position += dx;

            stepper.steppers[0].VelocityLimit = speed;
            derive += Goal_Position - (int)Goal_Position;
            Console.WriteLine("La dérive cumulée est de {0}", derive);
            if (derive >= 1)
            {
                Goal_Position += 1;
                derive = 1 - (int)derive;
            }
            else if (derive <= -1)
            {
                Goal_Position += -1;
                derive = (int)derive + 1;
            }

            // Don't know why but abs(dx) must be higher than 1 to work
            if (Math.Abs(dx) < 1)
            {
                System.Threading.Thread.Sleep((int)(dt * 1000));
            }
            else
            {
                stepper.steppers[0].TargetPosition = (int)Goal_Position;
            }
        }
        static bool isRunning()
        {
            return stepper.steppers[0].Stopped;
        }
        static void stopStepper()
        {
            stepper.steppers[0].Engaged = false;
            //close the Stepper object
            stepper.close();
            //set the object to null to get it out of memory
            stepper = null;
        }
        static void load_trajectory()
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\lcort_000\Desktop\coords" + motor_id.ToString() + "_1.txt");
            foreach (string line in lines)
            {
                liste.Add(Convert.ToDouble(line.Replace('.', ',')));
            }


        }
    }
}
