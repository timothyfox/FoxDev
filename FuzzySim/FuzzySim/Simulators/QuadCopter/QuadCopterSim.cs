/*
 *              ===== Harrier Simulator ===== 
 * 
 *  Original Simulation design:         Robert Cox [C++]
 *          C# Version ReWrite:         Timothy Fox
 *                        Date:         September 2012
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
        
            QuadCopter = new SimVars.HarrierVars();

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

            QuadCopter.Mass = 5655 + QuadCopter.Fuel;  // 5655 + fuel

            QuadCopter.Boom = false;
            QuadCopter.Landed = false;

            #endregion

            State = SimulatorStateEnum.Initialised;

            return new SimFrame();
        }

        private void InitDifficulty()
        {
            #region Sets up the Harrier Simulator depending on difficulty

            QuadCopter.PlaneImage = SpriteList.Harrier;

            if (Difficulty == SimDifficultyEnum.Easy)
            {

                QuadCopter.ShipImage = SpriteList.ShipEnterprise;

                QuadCopter.GroundEffect = false;   

                QuadCopter.RelativeXVel = 0; // ship air speed
                QuadCopter.MaxWindGust = 0; // max wind gusts
                QuadCopter.shipSpeed = 0;

                QuadCopter.Fuel = 900;  // fuel reserve
                QuadCopter.XVel = 4;  // start x speed
                QuadCopter.YVel = -5;  // start y speed

                QuadCopter.CarrierHeightPixels = 163;  // size of png image
                QuadCopter.CarrierWidthPixels = 837;
                QuadCopter.CarrierLandingHeightPixels = 50;

                QuadCopter.CarrierImageX = 0;       // Where to blit the carrier in pixels
     

                QuadCopter.UnsafeX1 = 4;           // safe to land here meters
                QuadCopter.SafeX2 = 323 + QuadCopter.UnsafeX1;           // safe to land here meters

                QuadCopter.UnsafeY1 = 0;           // in meters
                QuadCopter.SafeY2 = QuadCopter.CarrierLandingHeightPixels * QuadCopter.PixelsToMeters;      // in meters
                QuadCopter.SafeYDraw = QuadCopter.SafeY2 - 4;
                QuadCopter.UnsafeY3 =  0;           // in meters

                QuadCopter.MidSafeX = (QuadCopter.UnsafeX1 + QuadCopter.SafeX2) / 2; // calc middle of landing platform

                QuadCopter.UnFueledMass = 5942;    // kg
                QuadCopter.MaxForce = 84500;       // newton

                QuadCopter.Throttle = 74;

            }

            if (Difficulty == SimDifficultyEnum.Medium)
            {
                QuadCopter.ShipImage = SpriteList.ShipAustralia;

                QuadCopter.GroundEffect = true; 

                QuadCopter.RelativeXVel = 12; // ship air speed

                QuadCopter.WindSpeed = 5; //Wind speed
                QuadCopter.MaxWindGust = 5; // max wind gusts

                QuadCopter.Fuel = 900;  // fuel reserve
                QuadCopter.XVel = 27;  // start x speed
                QuadCopter.YVel = -5;  // start y speed
                QuadCopter.shipSpeed = 12;

                QuadCopter.CarrierHeightPixels = 112;  // size of png image
                QuadCopter.CarrierWidthPixels = 575;
                QuadCopter.CarrierLandingHeightPixels = 60;

                QuadCopter.CarrierImageX = 120;       // Where to blit the carrier in pixels
                
                QuadCopter.UnsafeX1 = 8 + QuadCopter.CarrierImageX * QuadCopter.PixelsToMeters;           // safe to land here meters
                QuadCopter.SafeX2 = 190 + QuadCopter.UnsafeX1;           // safe to land here meters

                QuadCopter.UnsafeY1 = 0;           // in meters
                QuadCopter.SafeY2 = QuadCopter.CarrierLandingHeightPixels * QuadCopter.PixelsToMeters;      // in meters
                QuadCopter.SafeYDraw = QuadCopter.SafeY2 - 4;                 
                QuadCopter.UnsafeY3 =0;           // in meters

                QuadCopter.MidSafeX = (QuadCopter.UnsafeX1 + QuadCopter.SafeX2) / 2; // calc middle of landing platform

                QuadCopter.UnFueledMass = 5942;    // kg
                QuadCopter.MaxForce = 84500;       // newton

                QuadCopter.Throttle = 84;
            }

            if (Difficulty == SimDifficultyEnum.Hard)
            {
                QuadCopter.ShipImage = SpriteList.ShipAustralia;

                QuadCopter.GroundEffect = true;

                QuadCopter.RelativeXVel = 20; // ship air speed

                QuadCopter.WindSpeed = 15; //Wind speed
                QuadCopter.MaxWindGust = 15; // max wind gusts


                QuadCopter.Fuel = 100;  // fuel reserve
                QuadCopter.XVel = 32;  // start x speed
                QuadCopter.YVel = -1;  // start y speed
                QuadCopter.shipSpeed = 20;

                QuadCopter.CarrierHeightPixels = 112;  // size of png image
                QuadCopter.CarrierWidthPixels = 575;
                QuadCopter.CarrierLandingHeightPixels = 60;

                QuadCopter.CarrierImageX = 120;       // Where to blit the carrier in pixels

                QuadCopter.UnsafeX1 = 8 + QuadCopter.CarrierImageX * QuadCopter.PixelsToMeters;           // safe to land here meters
                QuadCopter.SafeX2 = 190 + QuadCopter.UnsafeX1;           // safe to land here meters

                QuadCopter.UnsafeY1 = 0;           // in meters
                QuadCopter.SafeY2 = QuadCopter.CarrierLandingHeightPixels * QuadCopter.PixelsToMeters;      // in meters
                QuadCopter.SafeYDraw = QuadCopter.SafeYDraw = QuadCopter.SafeY2 - 4; 
                QuadCopter.UnsafeY3 = 0;           // in meters

                QuadCopter.MidSafeX = (QuadCopter.UnsafeX1 + QuadCopter.SafeX2) / 2; // calc middle of landing platform

                QuadCopter.UnFueledMass = 5700 + 3500;    // kg
                QuadCopter.MaxForce = 106000;       // newton

                QuadCopter.Throttle = 89;
            }

            if (Difficulty == SimDifficultyEnum.VeryHard)
            {
                QuadCopter.ShipImage = SpriteList.ShipAnzac;

                QuadCopter.GroundEffect = true;

                QuadCopter.RelativeXVel = 22; // ship air speed

                QuadCopter.WindSpeed = 10; //Wind speed
                QuadCopter.MaxWindGust = 20; // max wind gusts


                QuadCopter.Fuel = 80;  // fuel reserve
                QuadCopter.XVel = 46;  // start x speed
                QuadCopter.YVel = 2;  // start y speed
                QuadCopter.shipSpeed = 22;

                QuadCopter.CarrierHeightPixels = 74;  // size of png image
                QuadCopter.CarrierWidthPixels = 309;
                QuadCopter.CarrierLandingHeightPixels = 27;

                QuadCopter.CarrierImageX = 300;       // Where to blit the carrier in pixels

                QuadCopter.UnsafeX1 = 2 + QuadCopter.CarrierImageX * QuadCopter.PixelsToMeters;     // safe to land here meters
                QuadCopter.SafeX2 = 25 + QuadCopter.UnsafeX1;           // safe to land here meters

                QuadCopter.UnsafeY1 = 0;           // in meters
                QuadCopter.SafeY2 = QuadCopter.CarrierLandingHeightPixels * QuadCopter.PixelsToMeters;      // in meters
                QuadCopter.SafeYDraw = QuadCopter.SafeYDraw = QuadCopter.SafeY2 - 4; 
                QuadCopter.UnsafeY3 = 15;           // in meters

                QuadCopter.MidSafeX = (QuadCopter.UnsafeX1 + QuadCopter.SafeX2) / 2; // calc middle of landing platform

                QuadCopter.UnFueledMass = 5700 + 3500;    // kg
                QuadCopter.MaxForce = 106000;       // newton

                QuadCopter.Throttle = 89;
            }

            QuadCopter.InitSpeedKnots = QuadCopter.shipSpeed*QuadCopter.ConvertKnotsToMps;
            QuadCopter.X = 15;
            QuadCopter.Y = 170; // harrier location (init y=4,000m)
            QuadCopter.XVel = QuadCopter.InitSpeedKnots * QuadCopter.ConvertKnotsToMps; // convert knots to meters per second
            QuadCopter.ThrustVector = 92;

            QuadCopter.MaxLandingSpeedY = 3.4;       // 3 meters per second max
            QuadCopter.MaxLandingSpeedX = 3.4;       // 3 meters per second max
            
            #endregion
        }

        public override SimFrame DrawTurn(Vec2 scale) //scale provides the height (Y)
        {
            double height = scale.Y;

            QuadCopter.CarrierImageY = scale.Y - QuadCopter.CarrierHeightPixels - 28;

            SimFrame ret = new SimFrame
                            {
                                Error = false, 
                                ToBeDrawn = true, 
                                BackGround = Color.FromArgb(255, 0, 255, 255),
                                Renderables = new List<Drawable> {}
                            };

            AddTracePathBlip(ret);

            ret.AddRenderable((new Sprite("ship")
            {
                Picture = QuadCopter.ShipImage,
                Position = new Vec2(QuadCopter.CarrierImageX, QuadCopter.CarrierImageY)
            }
                    ));

            DrawLines(ret);

            CheckForFinishText(ret);

            if (Globals.DrawTracePath)
                DrawTracePath(ret);

            ret.AddRenderable(new Sprite("hs")
                                  {
                                      Picture = QuadCopter.PlaneImage,
                                      Position = new Vec2(ToX(QuadCopter.X-8),ToY(QuadCopter.Y+4)),
                                      Scale = new Vec2(1.5,1.5)

                                  }
                                );
            

            
            DrawText(30, ref ret);
            
            if(Globals.DrawDebug)
                DrawDebug(ret);

            ret.TracePath = TracePath;

            return ret;
        }

        private void AddTracePathBlip(SimFrame ret)
        {
            if (Globals.TurnCount%20 == 0)
                TracePath.Add(ret.PlotLocation(new Vec2(ToX(QuadCopter.X - 8), ToY(QuadCopter.Y + 4)), Color.Firebrick));
        }

        private void CheckForFinishText(SimFrame ret)
        {
            if(QuadCopter.Landed)
                if (QuadCopter.Boom)
                {
                    
                    ret.AddTextWithShadow("Failure", "FAIL", new Vec2(150, 150), 100 , new SolidBrush(Color.Red), new SolidBrush(Color.DimGray));
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
            Move2DInDirHS(QuadCopter.X, QuadCopter.Y, DegToRad(270 - QuadCopter.ThrustVector), QuadCopter.Throttle / 4, ref xx, ref yy);
            
            ret.AddTextWithShadow("debug_TV", String.Format("{0}", QuadCopter.ThrustVector.ToString("F")), new Vec2(ToX(QuadCopter.X+1), ToY(QuadCopter.Y-5)), 10, colour, shadow);
            ret.AddTextWithShadow("debug_Thrust", String.Format("{0}", QuadCopter.Throttle.ToString("F")), new Vec2(ToX(xx+2), ToY(yy)), 10, colour, shadow);
            
        }

        private void DrawLines(SimFrame ret)
        {
            double xx = 0, yy = 0;

            float throttleThickness = (float)((((QuadCopter.Throttle) - 60)/40) * 8f);

            //throttleThickness = 0.05f; 

            if (throttleThickness < .05f)
                throttleThickness = .05f; 

            Move2DInDirHS(QuadCopter.X, QuadCopter.Y, DegToRad(270 - QuadCopter.ThrustVector), QuadCopter.Throttle/4, ref xx, ref yy);

            ret.AddRenderable(new Line(new Vec2(ToX(QuadCopter.X+1), ToY(QuadCopter.Y-2)), new Vec2(ToX(xx+1), ToY(yy)),
                                       new SolidBrush(Color.Red), throttleThickness) { Id = "ThrustLine" });

            ret.AddRenderable(new Line(new Vec2(ToX(QuadCopter.UnsafeX1), ToY(QuadCopter.SafeYDraw + 1)),
                                       new Vec2(ToX(QuadCopter.SafeX2), ToY(QuadCopter.SafeYDraw + 1)), new SolidBrush(Color.Red), 3f)
                                  {Id = "LandingLine"});

            ret.AddRenderable(new Line(new Vec2(ToX(QuadCopter.MidSafeX - 3), ToY(QuadCopter.SafeYDraw + 1)),
                                       new Vec2(ToX(QuadCopter.MidSafeX + 3), ToY(QuadCopter.SafeYDraw + 1)),
                                       new SolidBrush(Color.Green), 3f) {Id = "MiddleSafe"});
        }

        private void DrawText(int x, ref  SimFrame ret)
        {
            Brush colour = new SolidBrush(Color.Black);
            Brush shadow = new SolidBrush(Color.White);

            ret.AddTextWithShadow("location", String.Format("X, Y: \t\t\t {0}, {1}", QuadCopter.X.ToString("F"), QuadCopter.Y.ToString("F")), new Vec2(x, 20), colour, shadow);
            ret.AddTextWithShadow("velocity", String.Format("X-Vel, Y-Vel: \t\t {0}, {1}", QuadCopter.XVel.ToString("F"), QuadCopter.YVel.ToString("F")), new Vec2(x, 40), colour, shadow);
            ret.AddTextWithShadow("consumption", String.Format("Secs, Fuel: \t\t {0}, {1}", ((double)Globals.TurnCount / QuadCopter.TickPerSecond).ToString("F"), QuadCopter.Fuel.ToString("F")), new Vec2(x, 60), colour, shadow);
            ret.AddTextWithShadow("physics", String.Format("Mass, Throttle: \t\t {0}, {1}", QuadCopter.Mass.ToString("F"), QuadCopter.Throttle.ToString("F")), new Vec2(x, 80), colour, shadow);
            ret.AddTextWithShadow("wind", String.Format("ThrustVec, WindGust: \t {0}, {1}", QuadCopter.ThrustVector.ToString("F"), QuadCopter.WindSpeed.ToString("F")), new Vec2(x, 100), colour, shadow);
            ret.AddTextWithShadow("relative", String.Format("Speed Relative to Ship: \t  {0}", QuadCopter.RelativeXVel.ToString("F")), new Vec2(x, 120), colour, shadow);
            ret.AddTextWithShadow("shipsSpeed", String.Format("Ship Speed knots (mps) \t  {0}, {1}", QuadCopter.shipSpeed.ToString("F"), (QuadCopter.shipSpeed*QuadCopter.ConvertKnotsToMps).ToString("F")), new Vec2(x, 140), colour, shadow);
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

            if (QuadCopter.Throttle <= 0)
                QuadCopter.Throttle = 0;

            if (QuadCopter.Throttle > 100)
                QuadCopter.Throttle = 100;

            //weigh
            QuadCopter.Mass = QuadCopter.UnFueledMass + QuadCopter.Fuel;

            QuadCopter.YVel = QuadCopter.YVel - 9.8 / QuadCopter.TickPerSecond;

            double tfuel = (QuadCopter.MaxFuelIn1Sec - QuadCopter.MinFuelIn1Sec) * QuadCopter.Throttle / 100 + QuadCopter.MinFuelIn1Sec; //Now Burn Fuel
            QuadCopter.Fuel = QuadCopter.Fuel - tfuel / QuadCopter.TickPerSecond;
            if (QuadCopter.Fuel < 0) QuadCopter.Fuel = 0;

            QuadCopter.Force = QuadCopter.Throttle * QuadCopter.MaxForce / 100 * 0.95; // 0.95 allows for cold air bleed
            if (QuadCopter.Fuel <= 0) QuadCopter.Force = 0;

            // compute new x and y velocity
            double xx = 0, yy = 0;
            Move2DInDirHS(0, 0, DegToRad(270 - QuadCopter.ThrustVector), QuadCopter.Force, ref xx, ref yy);
            xx = xx * -1;
            yy = yy * -1;
            QuadCopter.YVel = QuadCopter.YVel + (yy / QuadCopter.Mass) / QuadCopter.TickPerSecond;  // thrust vector and throttle
            QuadCopter.XVel = QuadCopter.XVel + (xx / QuadCopter.Mass) / QuadCopter.TickPerSecond;

            if (QuadCopter.GroundEffect && QuadCopter.Y <= 16 + QuadCopter.SafeY2) // ground effect
            {
                // 16 meters when ground efect happens
                // max ground effect is 0.5 m/s acceleration
                double ge = apportionI(0 + QuadCopter.SafeY2, 16 + QuadCopter.SafeY2, QuadCopter.Y, 0, 0.5) / QuadCopter.TickPerSecond;
                QuadCopter.YVel = QuadCopter.YVel - ge / QuadCopter.TickPerSecond;
            }

            // adjust for base wind speed
            double wind_inKnots = QuadCopter.WindSpeed; // wind and ship speed //Form5->Edit6->Text.ToIntDef(0); 
            double wind_inMps = wind_inKnots * QuadCopter.ConvertKnotsToMps;
            double frictionHarrier = 0.013; // very dubious source for this
            double wind_effect = wind_inMps * frictionHarrier;
            QuadCopter.XVel = QuadCopter.XVel - wind_effect / QuadCopter.TickPerSecond;

            // adjust for wind gusts

            QuadCopter.WindSpeed = QuadCopter.MaxWindGust + (double)new Random().Next(100) / 50 + (Globals.TurnCount % 31 - 16) / 4.0;
            if (QuadCopter.WindSpeed < 0) QuadCopter.WindSpeed = 0;
            if (QuadCopter.WindSpeed > QuadCopter.MaxWindGust) QuadCopter.WindSpeed = QuadCopter.MaxWindGust;

            double windGust_inMps = QuadCopter.MaxWindGust * QuadCopter.ConvertKnotsToMps;
            double windGust_effect = windGust_inMps * frictionHarrier;
            QuadCopter.XVel = QuadCopter.XVel - windGust_effect / QuadCopter.TickPerSecond;


            // adjust for ship speed
            QuadCopter.X = QuadCopter.X - (QuadCopter.shipSpeed * QuadCopter.ConvertKnotsToMps / QuadCopter.TickPerSecond);

            QuadCopter.RelativeXVel = QuadCopter.XVel - QuadCopter.shipSpeed * QuadCopter.ConvertKnotsToMps;

            // compute new position
            QuadCopter.Y = QuadCopter.Y + QuadCopter.YVel / QuadCopter.TickPerSecond;
            QuadCopter.X = QuadCopter.X + QuadCopter.XVel / QuadCopter.TickPerSecond;


            double sh = 0;
            bool b = SafeZoneHS(QuadCopter.X, ref sh);
            if (QuadCopter.Y <= sh + 3)
            {
                // aircraft has Landed
                QuadCopter.Y = sh + 3;
                if (!b)
                {
                    QuadCopter.PlaneImage = SpriteList.SpaceShip_Explode;

                    QuadCopter.Boom = true;
                    QuadCopter.Landed = true;
                    QuadCopter.Y = sh + 3;
                    QuadCopter.Throttle = 0;
                    QuadCopter.ThrustVector = 0;
                    State = SimulatorStateEnum.Complete;
                    
                    Fail();

                    return;
                }
                //Failed Landing
                if (Math.Abs(QuadCopter.RelativeXVel) > QuadCopter.MaxLandingSpeedX || Math.Abs(QuadCopter.YVel) > QuadCopter.MaxLandingSpeedY)
                {
                    QuadCopter.PlaneImage = SpriteList.SpaceShip_Explode;

                    QuadCopter.Boom = true;
                    QuadCopter.Landed = true;
                    QuadCopter.Y = sh + 3;
                    QuadCopter.Throttle = 0;
                    QuadCopter.ThrustVector = 0;
                    
                    State = SimulatorStateEnum.Complete;

                    Fail();

                    return;
                }

                // Success we did not blow up
                QuadCopter.Landed = true;
                QuadCopter.PlaneImage = SpriteList.HarrierLanded;
                
                State = SimulatorStateEnum.Complete;

                QuadCopter.Y = sh + 3;
                QuadCopter.Throttle = 0;
                QuadCopter.ThrustVector = 0;
                
                //drawHS();
                
                return;
            }

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
        double lerp(double t, double a,  double b)
        {
            return a + t*(b - a);
        }

        #endregion 
    }
}
