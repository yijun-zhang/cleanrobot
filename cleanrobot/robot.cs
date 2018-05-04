using System;
using System.Collections;
using System.Collections.Generic;

namespace cleanrobot
{  //think of robot ,it doesnot khnow the room's size and shape. and it does not khnow his position. we can think it's initial point is (0,0).
    class robot
    {
        public IDictionary<string, int> points = new Dictionary<string, int>();//all points with status( key=x,y value:0:unkhnown,1:cleaned,2:blocked)
        public int x = 0,y = 0;
        public int direction = 0; //(x,y,-x,-y) total is 4 directions 0,1,2,3
        public int unkhnown_poins_cnt = 0; //all points count with status=0;
        public ArrayList path = new ArrayList();//path points keys list
        public ArrayList allpath = new ArrayList();//all the path points keys list include come back points 
        public int[,] room; //only be used by move() api,for detect if the point is blocked 
        public int movetimes = 0, lefttimes = 0, righttimes = 0, cleantimes = 0; //how many actions does robot take to clean the room 
        //tow consturct function 
        public robot()//think of robot's position is always (0,0) and initial dirrection is 0, it is not necessary putting robot on point(x,y) with direction
        { }
        ////put robot on point(x,y) with direction
        public robot(int x, int y, int direction)
        {
            this.x = x;
            this.y = y;
            this.direction = direction;
        }
        public void cleanroom()
        {   //add initial points and four side unkhnown points to all points Dictionary if the point does not exist
            clean();
            addFivePoints();
            string[] sidepoint = new string[4]; //0 xside,1:yside 2:-xside 3:-yside
            int todirection, relativedir; //next ready to this direction;
            Boolean goToLastPoint;
            while (unkhnown_poins_cnt > 0)
            {
                getsidepoints(ref sidepoint);
                //check next point statue ,then decide how to act
                if (points[sidepoint[this.direction]] == 0)
                {
                    //if can move then 1:add path as last and movein, 2:clean and set this point statue=1 3:add side points to poins collection if point does not exist;   
                    if (move())
                    {
                        path.Add(this.x.ToString() + "," + this.y.ToString());
                        allpath.Add(this.x.ToString() + "," + this.y.ToString());
                        setNewPosition();
                        clean();//api
                        unkhnown_poins_cnt--;// robot's point is cleaned
                        addFivePoints();

                    }
                    else// 1:change that point with statue=2
                    {
                        points[sidepoint[this.direction]] = 2;
                        unkhnown_poins_cnt--;//  point is blocked
                    }
                }
                else //only turn to side with statue=0 otherwise go to lastpoint
                {
                    goToLastPoint = false;
                    relativedir = 1;
                    todirection = (this.direction + 1) % 4; //left

                    if (points[sidepoint[todirection]] != 0)
                    {
                        relativedir = 3;
                        todirection = (this.direction + 3) % 4; //right

                        if (points[sidepoint[todirection]] != 0)
                        {
                            relativedir = 2;
                            todirection = (this.direction + 2) % 4;//back

                            if (points[sidepoint[todirection]] != 0)
                            {
                                relativedir = HowToGoLastpoint(); //lastpoint
                                goToLastPoint = true;

                            }
                        }
                    }
                    // robot  action
                    Actionby(relativedir, this.direction);
                    this.direction = (this.direction + relativedir) % 4;
                    if (goToLastPoint)
                    {
                        move();
                        //delete last point of path
                        int pathCnt = path.Count;
                        path.RemoveAt(pathCnt - 1);
                        //record allpath
                        allpath.Add(this.x.ToString() + "," + this.y.ToString());
                        //set new position
                        setNewPosition();
                    }
                }
            }
            allpath.Add(this.x.ToString() + "," + this.y.ToString()); //last point
        }

        private void addFivePoints()
        {
            string key = this.x.ToString() + "," + this.y.ToString();
            if (!points.ContainsKey(key)) points.Add(key, 1); else points[key] = 1; //robot's point is cleaned
            string[] sidepoint = new string[4];
            getsidepoints(ref sidepoint);
            if (!points.ContainsKey(sidepoint[0])) { points.Add(sidepoint[0], 0); unkhnown_poins_cnt++; }
            if (!points.ContainsKey(sidepoint[1])) { points.Add(sidepoint[1], 0); unkhnown_poins_cnt++; }
            if (!points.ContainsKey(sidepoint[2])) { points.Add(sidepoint[2], 0); unkhnown_poins_cnt++; }
            if (!points.ContainsKey(sidepoint[3])) { points.Add(sidepoint[3], 0); unkhnown_poins_cnt++; }
        }

        private void setNewPosition()//move a step  
        {
            switch (this.direction)
            {
                case 0:
                    this.x = this.x + 1;
                    break;
                case 1:
                    this.y = this.y + 1;
                    break;
                case 2:
                    this.x = this.x - 1;
                    break;

                default:
                    this.y = this.y - 1;
                    break;
            }

        }
        private void getsidepoints(ref string[] sidepoint)
        {
            sidepoint[0] = (this.x + 1).ToString() + "," + this.y.ToString();
            sidepoint[1] = (this.x).ToString() + "," + (this.y + 1).ToString();
            sidepoint[2] = (this.x - 1).ToString() + "," + this.y.ToString();
            sidepoint[3] = (this.x).ToString() + "," + (this.y - 1).ToString();
        }
        private void Actionby(int todir, int ordir)
        {
            if (todir == (ordir + 1) % 4)
            {
                turnleft();
            }
            if (todir == (ordir + 3) % 4)
            {
                turnright();
            }
            if (todir == (ordir + 2) % 4)
            {
                turnleft();
                turnleft();
            }
        }

        private int HowToGoLastpoint() //get relative todir by robot direction+ current point and last point  ;
        {
            int pathCnt = path.Count - 1;
            string[] two = path[pathCnt].ToString().Split(',');
            int x1 = Convert.ToInt32(two[0]);
            int y1 = Convert.ToInt32(two[1]);
            if (x1 - x == 1)
            {
                return (4 - this.direction) % 4;
            }

            else if (y1 - y == 1)
            {
                return (5 - this.direction) % 4;
            }
            else if (x1 - x == -1)
            {
                return (6 - this.direction) % 4;
            }
            else
            {
                return (7 - this.direction) % 4;
            }
        }
        //4 api here 
        private bool move()
        {
            movetimes++;
            //for test code 
            int x1 = this.x;
            int y1 = this.y;
            if (this.direction == 0) x1 = x1 + 1;
            if (this.direction == 1) y1 = y1 + 1;
            if (this.direction == 2) x1 = x1 - 1;
            if (this.direction == 3) y1 = y1 - 1;
            if (room[x1, y1] == 0) return true; else return false;
        }
        private void clean() { cleantimes++; }
        private void turnleft() { lefttimes++; }
        private void turnright() { righttimes++; }
    }
}
