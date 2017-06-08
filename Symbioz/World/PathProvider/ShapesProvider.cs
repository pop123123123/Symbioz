
using Symbioz.Core.Startup;
using Symbioz.DofusProtocol.Messages;
using Symbioz.Enums;
using Symbioz.Network.Clients;
using Symbioz.Providers;
using Symbioz.World.Models.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symbioz.World.PathProvider
{
    /// <summary>
    /// Dofus 2.0 Shapes & MapCells Providers 
    /// </summary>
    public class ShapesProvider
    {
        #region Provider
        public delegate List<short> ShapeMethodDel(short startcell, short entitycell, short radius);
        public static Dictionary<char, ShapeMethodDel> Shapes = new Dictionary<char, ShapeMethodDel>();
        [StartupInvoke(StartupInvokeType.Others)]
        public static void Initialize()
        {
            foreach (var type in Assembly.GetAssembly(typeof(ShapesProvider)).GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    var attribute = method.GetCustomAttribute(typeof(Shape)) as Shape;
                    if (attribute != null)
                    {
                        Shapes.Add(attribute.ShapeIdentifier, (ShapeMethodDel)method.CreateDelegate(typeof(ShapeMethodDel)));
                    }
                }
            }
        }
        public static List<short> Handle(char shapetype, short startcell, short entitycell, short radius)
        {
            var shape = Shapes.FirstOrDefault(x => x.Key == shapetype);
            if (shape.Value != null)
                return shape.Value(startcell, entitycell, radius);
            else
            {

                Logger.Log(shapetype + " shape is not handled");
                return new List<short>();
            }
        }
        #endregion
        #region Simple
        public static List<short> GetRightCells(short startcell, short movedcellamount)
        {
            var list = new List<short>();
            for (short i = 1; i < (movedcellamount + 1); i++)
            {
                list.Add((short)(startcell + i));
            }
            return list;
        }
        public static List<short> GetLeftCells(short startcell, short movedcellamount)
        {
            var list = new List<short>();
            for (short i = 1; i < (movedcellamount + 1); i++)
            {
                list.Add((short)(startcell - i));
            }
            return list;
        }
        public static List<short> GetUpCells(short startcell, short movedcellamount)
        {
            var list = new List<short>();
            for (short i = 1; i < (movedcellamount + 1); i++)
            {
                list.Add((short)(startcell - 28 * i));
            }
            return list;
        }
        public static List<short> GetDownCells(short startcell, short movedcellamount)
        {
            var list = new List<short>();
            for (short i = 1; i < (movedcellamount + 1); i++)
            {
                list.Add((short)(startcell + 28 * i));
            }
            return list;
        }
        #endregion
        #region FrontDownRight&Left
        public static List<short> GetFrontDownLeftCells(short startcell, short movedcellamout)
        {
            var list = new List<short>();
            short currentCell = startcell;
            for (int i = 0; i < movedcellamout; i++)
            {
                if ((currentCell / 14)%2 == 0)//Si rangée paire, 13, sinon 14
                {
                    currentCell += 13;
                }
                else
                {
                    currentCell += 14;
                }
                list.Add(currentCell);
            }
            return list;
        }
        public static List<short> GetFrontDownRightCells(short startcell, short movedcellamout)
        {
            var list = new List<short>();
            short currentCell = startcell;
            for (int i = 0; i < movedcellamout; i++)
            {
                if ((currentCell / 14) % 2 == 0)//Si rangée paire, 14, sinon 15
                {
                    currentCell += 14;
                }
                else
                {
                    currentCell += 15;
                }
                list.Add(currentCell);
            }
            return list;
        }
        #endregion
        #region FrontUpRight&Left
        public static List<short> GetFrontUpLeftCells(short startcell, short movedcellamout)
        {
            var list = new List<short>();
            short currentCell = startcell;
            for (int i = 0; i < movedcellamout; i++)
            {
                if ((currentCell / 14) % 2 == 0)//Si rangée paire, -15, sinon -14
                {
                    currentCell -= 15;
                }
                else
                {
                    currentCell -= 14;
                }
                list.Add(currentCell);
            }
            return list;
        }
        public static List<short> GetFrontUpRightCells(short startcell, short movedcellamout) // youston on a probleme cell 500
        {
            var list = new List<short>();
            short currentCell = startcell;
            for (int i = 0; i < movedcellamout; i++)
            {
                if ((currentCell / 14) % 2 == 0)//Si rangée paire, -14, sinon -13
                {
                    currentCell -= 14;
                }
                else
                {
                    currentCell -= 13;
                }
                list.Add(currentCell);
            }
            return list;
        }
        #endregion
        #region Areas
        public static List<short> GetThe4thsDiagonal(short startcell, short movedcellamount)
        {
            var list = new List<short>();
            list.AddRange(Verify(GetDownCells(startcell, movedcellamount)));
            list.AddRange(Verify(GetUpCells(startcell, movedcellamount)));
            list.AddRange(Verify(GetRightCells(startcell, movedcellamount)));
            list.AddRange(Verify(GetLeftCells(startcell, movedcellamount)));
            return list;
        }
        public static List<short> GetSquare(short startcell, bool containstartcell)
        {
            var list = new List<short>();
            list.AddRange(Verify(GetFrontDownLeftCells(startcell, 1)));
            list.AddRange(Verify(GetFrontDownRightCells(startcell, 1)));
            list.AddRange(Verify(GetFrontUpLeftCells(startcell, 1)));
            list.AddRange(Verify(GetFrontUpRightCells(startcell, 1)));
            list.AddRange(Verify(GetDownCells(startcell, 1)));
            list.AddRange(Verify(GetUpCells(startcell, 1)));
            list.AddRange(Verify(GetRightCells(startcell, 1)));
            list.AddRange(Verify(GetLeftCells(startcell, 1)));
            if (containstartcell)
                list.Add(startcell);
            return list;
        }
        #endregion
        #region Utils
        public static List<short> Verify(List<short> cells)
        {
            cells.RemoveAll(x => x < 0 || x > 560);
            return cells;
        }
        public static List<short> GetLineFromOposedDirection(short startcell, short movecellamount, DirectionsEnum direction)
        {
            switch (direction) // all good, directionfinderrework
            {
                case DirectionsEnum.DIRECTION_SOUTH_EAST:
                    return Verify(GetFrontUpLeftCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_SOUTH_WEST: // good
                    return Verify(GetFrontUpRightCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_NORTH_WEST:
                    return Verify(GetFrontDownRightCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_NORTH_EAST: // good
                    return Verify(GetFrontDownLeftCells(startcell, movecellamount));
                default:
                    return null;
            }
        }
        public static bool IsDiagonalDirection(DirectionsEnum direction)
        {
            if (direction == DirectionsEnum.DIRECTION_EAST || direction == DirectionsEnum.DIRECTION_NORTH ||
                direction == DirectionsEnum.DIRECTION_SOUTH || direction == DirectionsEnum.DIRECTION_WEST)
                return true;
            else
                return false;
        }
        public static List<short> GetLineFromDirection(short startcell, short movecellamount, DirectionsEnum direction)
        {
            switch (direction)
            {
                case DirectionsEnum.DIRECTION_EAST:
                    return Verify(GetRightCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_SOUTH_EAST:
                    return Verify(GetFrontDownRightCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_SOUTH:
                    return Verify(GetDownCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_SOUTH_WEST:
                    return Verify(GetFrontDownLeftCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_WEST:
                    return Verify(GetLeftCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_NORTH_WEST:
                    return Verify(GetFrontUpLeftCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_NORTH:
                    return Verify(GetUpCells(startcell, movecellamount));
                case DirectionsEnum.DIRECTION_NORTH_EAST:
                    return Verify(GetFrontUpRightCells(startcell, movecellamount));
                default:
                    return null;
            }
        }
        public static List<short> GetLineFromDirectionWithoutBounds(short startcell, short movecellamount, DirectionsEnum direction)
        {
            switch (direction)
            {
                case DirectionsEnum.DIRECTION_EAST:
                    return GetRightCells(startcell, movecellamount);
                case DirectionsEnum.DIRECTION_SOUTH_EAST:
                    return GetFrontDownRightCells(startcell, movecellamount);
                case DirectionsEnum.DIRECTION_SOUTH:
                    return GetDownCells(startcell, movecellamount);
                case DirectionsEnum.DIRECTION_SOUTH_WEST:
                    return GetFrontDownLeftCells(startcell, movecellamount);
                case DirectionsEnum.DIRECTION_WEST:
                    return GetLeftCells(startcell, movecellamount);
                case DirectionsEnum.DIRECTION_NORTH_WEST:
                    return GetFrontUpLeftCells(startcell, movecellamount);
                case DirectionsEnum.DIRECTION_NORTH:
                    return GetUpCells(startcell, movecellamount);
                case DirectionsEnum.DIRECTION_NORTH_EAST:
                    return GetFrontUpRightCells(startcell, movecellamount);
                default:
                    return null;
            }
        }
        #endregion
        #region DirectionHelper
        public static DirectionsEnum GetOposedDirection(DirectionsEnum direction)
        {
            switch (direction)
            {
                case DirectionsEnum.DIRECTION_EAST:
                    return DirectionsEnum.DIRECTION_WEST;
                case DirectionsEnum.DIRECTION_SOUTH_EAST:
                    return DirectionsEnum.DIRECTION_NORTH_WEST;
                case DirectionsEnum.DIRECTION_SOUTH:
                    return DirectionsEnum.DIRECTION_NORTH;
                case DirectionsEnum.DIRECTION_SOUTH_WEST:
                    return DirectionsEnum.DIRECTION_NORTH_EAST;
                case DirectionsEnum.DIRECTION_WEST:
                    return DirectionsEnum.DIRECTION_EAST;
                case DirectionsEnum.DIRECTION_NORTH_WEST:
                    return DirectionsEnum.DIRECTION_SOUTH_EAST;
                case DirectionsEnum.DIRECTION_NORTH:
                    return DirectionsEnum.DIRECTION_SOUTH;
                case DirectionsEnum.DIRECTION_NORTH_EAST:
                    return DirectionsEnum.DIRECTION_SOUTH_WEST;
                default:
                    throw new Exception("What is dat direction dude?");
            }
        }
        public static DirectionsEnum GetDirectionFromTwoCells(short firstcellid, short secondccellid) // first = caster
        {
            if (Verify(GetFrontDownLeftCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_SOUTH_WEST;
            if (Verify(GetFrontDownRightCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_SOUTH_EAST;
            if (Verify(GetFrontUpLeftCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_NORTH_WEST;
            if (Verify(GetFrontUpRightCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_NORTH_EAST;

            if (Verify(GetRightCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_EAST;
            if (Verify(GetLeftCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_WEST;
            if (Verify(GetUpCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_NORTH;
            if (Verify(GetDownCells(firstcellid, 10)).Contains(secondccellid))
                return DirectionsEnum.DIRECTION_SOUTH;

            return 0;
        }
        #endregion
        #region Shapes
        [Shape('U')]
        public static List<short> GetUShape(short startcell, short entitycell, short radius)
        {
            List<short> results = new List<short>();
            var direction = GetDirectionFromTwoCells(startcell, entitycell);
            var line = GetLineFromDirection(startcell, 2, direction);
            line.Remove(line.Last());
            line = GetSquare(line[0], true);
            results.Add(startcell);
            if (direction == DirectionsEnum.DIRECTION_SOUTH_WEST || direction == DirectionsEnum.DIRECTION_NORTH_EAST)
            {
                results.Add(line[1]);
                results.Add(line[2]);

            }
            if (direction == DirectionsEnum.DIRECTION_NORTH_WEST || direction == DirectionsEnum.DIRECTION_SOUTH_EAST)
            {
                results.Add(line[0]);
                results.Add(line[3]);
            }
            Verify(results);
            return results;
        }
        /// <summary>
        /// 'a' & 'A' differences?
        /// </summary>
        /// <param name="startcell"></param>
        /// <param name="entitycell"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        [Shape('A')]
        public static List<short> GetAShape(short startcell,short entitycell,short radius)
        {
            return GetaShape(startcell, entitycell, radius);
        }
        [Shape('a')]
        public static List<short> GetaShape(short startcell,short entitycell,short radius)
        {
            List<short> results = new List<short>();
            for (short i = 0; i < 559; i++)
            {
                results.Add(i);
            }
            return results;
        }
        [Shape('+')]
        public static List<short> GetPlusShape(short startcell, short entitycell, short radius)
        {
            List<short> results = new List<short>();
            results.Add(startcell);
            results.Add(Verify(GetLeftCells(startcell, 1))[0]);
            results.Add(Verify(GetRightCells(startcell, 1))[0]);
            results.Add(Verify(GetUpCells(startcell, 1))[0]);
            results.Add(Verify(GetDownCells(startcell, 1))[0]);
            Verify(results);
            return results;
        }
        [Shape('V')]
        public static List<short> GetVShape(short startcell, short entitycell, short radius)
        {
            List<short> results = new List<short>();
            results.Add(startcell);
            var direction = GetDirectionFromTwoCells(entitycell, startcell);
            var line = GetLineFromDirection(startcell, 1, direction);
            results.AddRange(line);
            switch (direction)
            {
                case DirectionsEnum.DIRECTION_SOUTH_EAST:
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_SOUTH_WEST));
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_NORTH_EAST));
                    break;
                case DirectionsEnum.DIRECTION_SOUTH_WEST:
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_SOUTH_EAST));
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_NORTH_WEST));
                    break;
                case DirectionsEnum.DIRECTION_NORTH_WEST:
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_SOUTH_WEST));
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_NORTH_EAST));
                    break;
                case DirectionsEnum.DIRECTION_NORTH_EAST:
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_NORTH_WEST));
                    results.AddRange(GetLineFromDirection(line[0], 1, DirectionsEnum.DIRECTION_SOUTH_EAST));
                    break;
            }

            Verify(results);
            return results;
        }
        [Shape('*')]
        public static List<short> GetStarShape(short startcell, short entitycell, short radius)
        {
            var cells = GetCShape(startcell, entitycell, radius);
            cells.AddRange(Verify(GetUpCells(startcell, radius)));
            cells.AddRange(Verify(GetDownCells(startcell, radius)));
            cells.AddRange(Verify(GetRightCells(startcell, radius)));
            cells.AddRange(Verify(GetLeftCells(startcell, radius)));
            Verify(cells);
            return cells.Distinct().ToList();
        }
        [Shape('G')]
        public static List<short> GetGShape(short startcell, short entitycell, short radius)
        {
            short shift = 1;
            if (radius >= 3)
            {
                for (int i = 3; i < radius + 1; i++)
                {
                    shift++;
                }
            }
            List<short> results = new List<short>();
            for (int i = 1; i < radius + 1; i++)
            {
                results.Add(startcell);
                results.Add((short)(startcell + i));
                results.AddRange(Verify(GetFrontDownLeftCells((short)(startcell + i), (short)(shift + i))));
                results.AddRange(Verify(GetFrontUpLeftCells((short)(startcell + i), (short)(shift + i))));
                results.AddRange(Verify(GetFrontDownRightCells((short)(startcell - i), (short)(shift + i))));
                results.AddRange(Verify(GetFrontUpRightCells((short)(startcell - i), (short)(shift + i))));
                results.AddRange(Verify(GetLeftCells((short)(startcell), (short)(i))));
                results.AddRange(Verify(GetDownCells((short)(startcell), (short)(i))));
                results.AddRange(Verify(GetUpCells((short)(startcell), (short)(i))));
            }
            Verify(results);
            return results.Distinct().ToList();
        }
        [Shape('Q')]
        public static List<short> GetQShape(short startcell, short entitycell, short radius)
        {
            List<short> results = new List<short>();
            results.AddRange(Verify(GetFrontDownLeftCells(startcell, radius)));
            results.AddRange(Verify(GetFrontDownRightCells(startcell, radius)));
            results.AddRange(Verify(GetFrontUpLeftCells(startcell, radius)));
            results.AddRange(Verify(GetFrontUpRightCells(startcell, radius)));
            Verify(results);
            return results;
        }
        [Shape('X')]
        public static List<short> GetCrossCells(short startcell, short entitycell, short radius)
        {
            List<short> results = new List<short>();
            results.Add(startcell);
            results.AddRange(Verify(GetFrontDownLeftCells(startcell, radius)));
            results.AddRange(Verify(GetFrontDownRightCells(startcell, radius)));
            results.AddRange(Verify(GetFrontUpLeftCells(startcell, radius)));
            results.AddRange(Verify(GetFrontUpRightCells(startcell, radius)));
            Verify(results);
            return results;
        }
        [Shape('L')]
        public static List<short> GeetLShape(short startcell, short entitycell, short radius)
        {
            var line = GetLineFromDirection(startcell, radius, GetDirectionFromTwoCells(entitycell, startcell));
            line.Add(startcell);
            Verify(line);
            return line;
        }
        [Shape('B')]
        public static List<short> GetBombLine(short startcell,short entitycell,short radius)
        {
            var line = GetLineFromDirection(startcell, radius, GetDirectionFromTwoCells(startcell, entitycell));
            line.Remove(line.Last());
            Verify(line);
            return line;
        }
        [Shape('C')]
        public static List<short> GetCShape(short startcell, short entitycell, short radius)
        {
            return Pathfinding.GetCircleCells(startcell, radius);
        }
        [Shape('T')]
        public static List<short> GetTShape(short startcell, short entitycell, short radius)
        {
            List<short> cells = new List<short>();
            cells.Add(startcell);
            var position = GetDirectionFromTwoCells(entitycell, startcell);
            switch (position)
            {
                case DirectionsEnum.DIRECTION_EAST:
                    return cells;
                case DirectionsEnum.DIRECTION_SOUTH_EAST:
                    cells.AddRange(Verify(GetFrontDownLeftCells(startcell, radius)));
                    cells.AddRange(Verify(GetFrontUpRightCells(startcell, radius)));
                    break;
                case DirectionsEnum.DIRECTION_SOUTH:
                    return cells;
                case DirectionsEnum.DIRECTION_SOUTH_WEST:
                    cells.AddRange(Verify(GetFrontUpLeftCells(startcell, radius)));
                    cells.AddRange(Verify(GetFrontDownRightCells(startcell, radius)));
                    break;
                case DirectionsEnum.DIRECTION_WEST:
                    break;
                case DirectionsEnum.DIRECTION_NORTH_WEST:
                    cells.AddRange(Verify(GetFrontUpRightCells(startcell, radius)));
                    cells.AddRange(Verify(GetFrontDownLeftCells(startcell, radius)));
                    break;
                case DirectionsEnum.DIRECTION_NORTH:
                    break;
                case DirectionsEnum.DIRECTION_NORTH_EAST:
                    cells.AddRange(Verify(GetFrontUpLeftCells(startcell, radius)));
                    cells.AddRange(Verify(GetFrontDownRightCells(startcell, radius)));
                    break;
            }
            Verify(cells);
            return cells;
        }
        public static List<short> GetCross1RadiusCells(short baseposition)
        {
            List<short> results = new List<short>();
            results.Add(Verify(GetFrontDownLeftCells(baseposition, 1))[0]);
            results.Add(Verify(GetFrontDownRightCells(baseposition, 1))[0]);
            results.Add(Verify(GetFrontUpLeftCells(baseposition, 1))[0]);
            results.Add(Verify(GetFrontUpRightCells(baseposition, 1))[0]);
            Verify(results);
            return results;
        }
        [Shape('P')]
        public static List<short> GetPShape(short startcell, short basecell, short radius)
        {
            return new List<short>() { startcell };
        }
        [Shape('O')]
        public static List<short> GetOShape(short startcell, short basecell, short radius)
        {
            List<short> results = new List<short>();
            /*System.Console.WriteLine(Verify(GetFrontDownLeftCells(startcell, 25))[5]);
            results.Add(Verify(GetFrontDownLeftCells(startcell, 5))[0]);
            System.Console.WriteLine(results[0]);
            //results.Add(Verify(GetFrontDownRightCells(startcell, 1))[radius]);
            //results.Add(Verify(GetFrontUpLeftCells(startcell, 1))[radius]);
            //results.Add(Verify(GetFrontUpRightCells(startcell, 1))[radius]);*/
            results.Add(startcell);
            return results;
        }
        #endregion
        #region MapBorder
        public static List<short> GetMapBorder(MapScrollType bordertype)
        {
            List<short> results = new List<short>();
            switch (bordertype)
            {
                case MapScrollType.TOP:
                    results.AddRange(GetLineFromDirection(0, 14, DirectionsEnum.DIRECTION_EAST));
                    results.AddRange(GetLineFromDirection(14, 14, DirectionsEnum.DIRECTION_EAST));
                    break;
                case MapScrollType.LEFT:
                    results.AddRange(GetLineFromDirection(14, 19, DirectionsEnum.DIRECTION_SOUTH));
                    results.AddRange(GetLineFromDirection(0, 19, DirectionsEnum.DIRECTION_SOUTH));
                    break;
                case MapScrollType.BOTTOM:
                    results.AddRange(GetLineFromDirection(546, 14, DirectionsEnum.DIRECTION_WEST));
                    results.AddRange(GetLineFromDirection(560, 14, DirectionsEnum.DIRECTION_WEST));
                    break;
                case MapScrollType.RIGHT:
                    results.AddRange(GetLineFromDirection(27, 19, DirectionsEnum.DIRECTION_SOUTH));
                    results.AddRange(GetLineFromDirection(13, 19, DirectionsEnum.DIRECTION_SOUTH));
                    break;
                default:
                    break;
            }
            return results;
        }
        #endregion
    }
}
