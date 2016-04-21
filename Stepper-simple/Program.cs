/* - Stepper simple -
 ****************************************************************************************
 * This simple example sets up a Stepper object, hooks the event handlers and opens it 
 * for device connections.  Once a Stepper is attached with a motor in motor 0 it will 
 * reset the motor to position 0 and then move the motor to position 2000, displaying the
 * event details to the console. For a more detailed example, see the Stepper-full 
 * example.
 * 
 * Please note that this example was designed to work with only one Phidget Stepper 
 * connected. 
 * For an example showing how to use two Phidgets of the same time concurrently, please see the
 * Servo-multi example in the Servo Examples.
 *
 * Copyright 2007 Phidgets Inc.  All rights reserved.
 * This work is licensed under the Creative Commons Attribution 2.5 Canada License. 
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/2.5/ca/
 */

using System;

using Move_cable;

namespace Stepper_simple
{
    class Program
    {
        
        static void Main(string[] args)
        {
            MyStepper step = new MyStepper(1);
            step.load();
            Console.ReadLine();
            MyStepper step2 = new MyStepper(2);
            MyStepper step3 = new MyStepper(3);
            //step2.load();
            Console.ReadLine();
            //step3.load();
            Console.ReadLine();
            
            int nbr_step = step.nbr_step();

            Console.WriteLine("Press a key to start...");
            Console.ReadLine();
            for (int i = 0; i < nbr_step-1; i++)
            {
                Console.WriteLine(i);
                step.run_step(i);
                //step2.run_step(i);
                //step3.run_step(i);
                //Ajouter dans la boucle while les autres moteurs
                while(step.stepper.steppers[0].Stopped == false) { }
                
            }
            step.close();
            //step2.close();
            //step3.close();
            Console.WriteLine("Press a key to end...");
            Console.ReadLine();

        }
    }
}

       
