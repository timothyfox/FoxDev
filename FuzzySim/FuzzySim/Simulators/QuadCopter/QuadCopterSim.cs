/*
 *              ===== Quadcopter Simulator ===== 
 * 
 *          C# Version        :         Timothy Fox
 *                        Date:         October 2012
 * 
*/

namespace FuzzySim.Simulators
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Core;
    using Rendering;


    class QuadCopterSim : AISimulator
    {
        public SimVars.QuadCopterVars QuadCopter;

        public override void Succeed()
        {
            throw new NotImplementedException();
        }

        public override void Fail()
        {

        }

        public override SimFrame Init(Vec2 rez)
        {
            TracePath = new List<PrintText>();

            #region Initializes the Harrier Simulator

            //set up the sea background
            /*            _sea = new Sprite("sea")
                        {
                            Picture = SpriteList.SeaSurface,
                            Scale = new Vec2(3, 3),

                        };*/

            QuadCopter = new SimVars.QuadCopterVars();

            QuadCopter.GroundEffect = false;

            QuadCopter.MetersToPixels = 2.5;
            QuadCopter.PixelsToMeters = 1 / QuadCopter.MetersToPixels;

            QuadCopter.TickPerSecond = 3;

            QuadCopter.MaxWindGust = 0;

            InitDifficulty();

            QuadCopter.Landed = false;
            QuadCopter.Boom = false;

            QuadCopter.MaxFuelIn1Sec = 0.5;
            QuadCopter.MinFuelIn1Sec = 0.1;


            QuadCopter.Boom = false;
            QuadCopter.Landed = false;

            #endregion

            State = SimulatorStateEnum.Initialised;

            return new SimFrame();
        }

        private void InitDifficulty()
        {
            #region Sets up the Harrier Simulator depending on difficulty


            if (Difficulty == SimDifficultyEnum.Easy)
            {


            }

            if (Difficulty == SimDifficultyEnum.Medium)
            {
            }

            if (Difficulty == SimDifficultyEnum.Hard)
            {
            }

            if (Difficulty == SimDifficultyEnum.VeryHard)
            {
            }


            #endregion
        }

        public override SimFrame DrawTurn(Vec2 scale) //scale provides the height (Y)
        {
            double height = scale.Y;


            SimFrame ret = new SimFrame
                            {
                                Error = false,
                                ToBeDrawn = true,
                                BackGround = Color.FromArgb(255, 0, 255, 255),
                                Renderables = new List<Drawable> { }
                            };

            return ret;
        }

        private void AddTracePathBlip(SimFrame ret)
        {
            if (Globals.TurnCount % 20 == 0)
                TracePath.Add(ret.PlotLocation(new Vec2(ToX(QuadCopter.X - 8), ToY(QuadCopter.Y + 4)), Color.Firebrick));
        }

        private void CheckForFinishText(SimFrame ret)
        {
            if (QuadCopter.Landed)
                if (QuadCopter.Boom)
                {

                    ret.AddTextWithShadow("Failure", "FAIL", new Vec2(150, 150), 100, new SolidBrush(Color.Red), new SolidBrush(Color.DimGray));
                }
                else
                {
                    ret.AddTextWithShadow("Success", "SAFE", new Vec2(150, 150), 100, new SolidBrush(Color.Green), new SolidBrush(Color.DimGray));
                }
        }




        private void DrawDebug(SimFrame ret)
        {
            Brush colour = new SolidBrush(Color.Red);
            Brush shadow = new SolidBrush(Color.White);

            double xx = 0, yy = 0;

        }

        private void DrawLines(SimFrame ret)
        {
            double xx = 0, yy = 0;

            ret.AddRenderable(new Line(new Vec2(ToX(QuadCopter.UnsafeX1), ToY(QuadCopter.SafeYDraw + 1)),
                                       new Vec2(ToX(QuadCopter.SafeX2), ToY(QuadCopter.SafeYDraw + 1)), new SolidBrush(Color.Red), 3f) { Id = "LandingLine" });

            ret.AddRenderable(new Line(new Vec2(ToX(QuadCopter.MidSafeX - 3), ToY(QuadCopter.SafeYDraw + 1)),
                                       new Vec2(ToX(QuadCopter.MidSafeX + 3), ToY(QuadCopter.SafeYDraw + 1)),
                                       new SolidBrush(Color.Green), 3f) { Id = "MiddleSafe" });
        }

        private void DrawText(int x, ref  SimFrame ret)
        {
            Brush colour = new SolidBrush(Color.Black);
            Brush shadow = new SolidBrush(Color.White);

            ret.AddTextWithShadow("Value", "0", new Vec2(x, 140), colour, shadow);
            ret.AddTextWithShadow("Value", "0", new Vec2(x, 140), colour, shadow);
        }

        public override SimFrame DrawTurn(SimFrame frame)
        {


            return frame;
        }

        /// <summary>
        /// Does all siome 
        /// </summary>
        public override void DoTurn()
        {
            #region Harrier Physics

            Globals.Controller.CalculateFuzzyLogic();

            if (QuadCopter.Landed)
            {
                State = SimulatorStateEnum.Complete;
                return;
            }

            return;


            #endregion
        }

        #region Harrier Physics Dependancies

        double ToX(double x)
        {
            return QuadCopter.MetersToPixels * x;
        }

        double ToY(double y)
        {
            double tmp = QuadCopter.MetersToPixels * y;
            return Globals.Simulator.FrameResolution.Y - tmp - 1;
        }


        /// <summary>
        /// returns true if in safe zone
        /// </summary>
        /// <param name="x"></param>
        /// <param name="safeHeight"></param>
        /// <returns></returns>
        bool SafeZoneHS(double x, ref double safeHeight)
        {
            //returns true if in safe zone
            if (x < QuadCopter.UnsafeX1)
            {
                safeHeight = QuadCopter.UnsafeY1;
                return false;
            }
            if (x > QuadCopter.SafeX2)
            {
                safeHeight = QuadCopter.UnsafeY3;
                return false;
            }

            safeHeight = QuadCopter.SafeY2;
            return true;
        }

        void Move2DInDirHS(double oldx, double oldz,
                 double yaw, double speed,
                 ref double resultx, ref double resultz)
        {
            double tmpx, tmpz;
            double newx, newz;

            /* set to origin */
            tmpx = 0;
            tmpz = speed; /* move us forward by speed */

            newx = (tmpz * (double)Math.Sin(yaw)) + (tmpx * (double)Math.Cos(yaw));
            newz = (tmpz * (double)Math.Cos(yaw)) - (tmpx * (double)Math.Sin(yaw));
            tmpx = newx;
            tmpz = newz;

            newx = tmpx + oldx;
            newz = tmpz + oldz;

            resultx = newx;
            resultz = newz;
        }

        double DegToRad(double deg)
        {
            return (deg * 3.14159 / 180.0);
        }


        /// <summary>
        /// This assumes that test val is in the range lowVal to highVal and finds its proportion
        /// it then returns the equivalent number in the range  returnRangeHigh to returnRangeLow
        /// this is identical to apportion except that is a proportion of 1-p
        /// </summary>
        /// <param name="lowVal"></param>
        /// <param name="highVal"></param>
        /// <param name="testVal"></param>
        /// <param name="returnRangeLow"></param>
        /// <param name="returnRangeHigh"></param>
        /// <returns></returns>
        double apportionI(double lowVal, double highVal, double testVal, double returnRangeLow, double returnRangeHigh)
        {
            double retv;
            if (highVal - lowVal == 0) return 0; // invalid equasion
            retv = (testVal - lowVal) / (highVal - lowVal); // retv now in range 0..1
            return lerp(1 - retv, returnRangeLow, returnRangeHigh);


        }

        /// <summary>
        ///  for lerp t is in the range 0 to 1 a=minval b=maxval
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        double lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        #endregion
    }
}
