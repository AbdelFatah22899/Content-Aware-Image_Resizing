using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ContentAwareResize
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public class ContentAwareResize
    {
        public struct coord
        {
            public int row;
            public int column;
        }
        //========================================================================================================
        //Your Code is Here:
        //===================
        /// <summary>
        /// Develop an efficient algorithm to get the minimum vertical seam to be removed
        /// </summary>
        /// <param name="energyMatrix">2D matrix filled with the calculated energy for each pixel in the image</param>
        /// <param name="Width">Image's width</param>
        /// <param name="Height">Image's height</param>
        /// <returns>BY REFERENCE: The min total value (energy) of the selected seam in "minSeamValue" & List of points of the selected min vertical seam in seamPathCoord</returns>
        int left, right, down, up;
        //int width_cur, height_cur;
        coord c=new coord();
        public void CalculateSeamsCost(int[,] energyMatrix, int Width, int Height, ref int minSeamValue, ref List<coord> seamPathCoord)
        {
            //throw new NotImplementedException();
            seamPathCoord = new List<coord>();
            for(int h=Height-1; h>=0; h--)
            {                                                  
                for(int w=Width-1; w>=0; w--)                //2, 3, 4, 5            ,
                {                                            //6, 7, 8, 9    7 ,12,11,11 
                    if (h == Height - 1)                     //1, 5, 3, 2    1 ,5 ,3 ,2
                        break; 
                    down =energyMatrix[h,w]+ energyMatrix[h+1,w];
                    if (w + 1 > Width - 1 && w - 1 < 0)
                        energyMatrix[h,w] = down;
                    else
                    {
                        if (w + 1 > Width - 1)
                        {
                            left = energyMatrix[h,w] + energyMatrix[h+1,w-1];
                            energyMatrix[h,w] = Math.Min(left, down);
                        }
                        else if (w - 1 < 0)
                        {
                            right = energyMatrix[h,w] + energyMatrix[h + 1,w+1];
                            energyMatrix[h,w] = Math.Min(right, down);
                        }
                        else
                        {
                            left = energyMatrix[h,w] + energyMatrix[h+1,w-1];
                            right = energyMatrix[h,w] + energyMatrix[h + 1,w+1];
                            energyMatrix[h,w] = Math.Min(down, Math.Min(right, left));
                        }
                    }
                }
            }


            minSeamValue = 20000;
            
            for (int w=0; w<Width; w++)
            {
                if (energyMatrix[0, w] < minSeamValue)
                {
                    minSeamValue = energyMatrix[0, w];
                    
                    c.row = 0;
                    c.column = w;

                    if (seamPathCoord.Count > 0)
                        seamPathCoord.Clear();
                    seamPathCoord.Add(c);
                }
            }

            for(int h=1; h<Height; h++)
            {

                c.row = h;
                if (c.column + 1 > Width - 1 && c.column - 1 < 0)
                {
                    seamPathCoord.Add(c);
                }
                else
                {
                    if (c.column == Width - 1)    //في اخر عمود
                    {
                        if (energyMatrix[h, c.column] >= energyMatrix[h, c.column - 1])
                        {
                            c.column -= 1;
                            seamPathCoord.Add(c);
                        }
                        else
                        {
                            seamPathCoord.Add(c);
                        }
                    }
                    else if (c.column == 0)
                    {
                        if (energyMatrix[h, c.column] >= energyMatrix[h, c.column + 1])
                        {
                            c.column += 1;
                            seamPathCoord.Add(c);
                        }
                        else
                        {
                            seamPathCoord.Add(c);
                        }
                    }
                    else
                    {
                        if (energyMatrix[h, c.column] <= energyMatrix[h, c.column - 1] && energyMatrix[h, c.column] <= energyMatrix[h, c.column + 1])
                        {
                            seamPathCoord.Add(c);
                        }
                        else if (energyMatrix[h, c.column - 1] <= energyMatrix[h, c.column] && energyMatrix[h, c.column - 1] <= energyMatrix[h, c.column + 1])
                        {
                            c.column -= 1;
                            seamPathCoord.Add(c);
                        }
                        else
                        {
                            c.column += 1;
                            seamPathCoord.Add(c);
                        }
                    }
                }
            }
        }

        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO 
        // *****************************************
        #region DON'TCHANGETHISCODE
        public MyColor[,] _imageMatrix;
        public int[,] _energyMatrix;
        public int[,] _verIndexMap;
        public ContentAwareResize(string ImagePath)
        {
            _imageMatrix = ImageOperations.OpenImage(ImagePath);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);
            int _height = _energyMatrix.GetLength(0);
            int _width = _energyMatrix.GetLength(1);
        }
        public void CalculateVerIndexMap(int NumberOfSeams, ref int minSeamValueFinal, ref List<coord> seamPathCoord)
        {
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);

            int minSeamValue = -1;
            _verIndexMap = new int[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    _verIndexMap[i, j] = int.MaxValue;

            bool[] RemovedSeams = new bool[Width]; 
            for (int j = 0; j < Width; j++)
                RemovedSeams[j] = false;

            for (int s = 1; s <= NumberOfSeams; s++)
            {
                CalculateSeamsCost(_energyMatrix, Width, Height, ref minSeamValue, ref seamPathCoord);
                minSeamValueFinal = minSeamValue;

                //Search for Min Seam # s
                int Min = minSeamValue;

                //Mark all pixels of the current min Seam in the VerIndexMap
                if (seamPathCoord.Count != Height)
                    throw new Exception("You selected WRONG SEAM");
                for (int i = Height - 1; i >= 0; i--)
                {
                        if (_verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] != int.MaxValue)
                        {
                            string msg = "overalpped seams between seam # " + s + " and seam # " + _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column];
                            throw new Exception(msg);
                        }
                        _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] = s;
                        //remove this seam from energy matrix by setting it to max value
                        _energyMatrix[seamPathCoord[i].row, seamPathCoord[i].column] = 100000;
                    
                    
                }

                //re-calculate Seams Cost in the next iteration again
            }
        }
        public void RemoveColumns(int NumberOfCols)
        {  
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);

            int minSeamValue = 0;
            List<coord> seamPathCoord = null;
            //CalculateSeamsCost(_energyMatrix,Width,Height,ref minSeamValue, ref seamPathCoord);
            CalculateVerIndexMap(NumberOfCols,ref minSeamValue,ref seamPathCoord);
                
            MyColor[,] OldImage = _imageMatrix;
            _imageMatrix = new MyColor[Height, Width - NumberOfCols];
            for (int i = 0; i < Height; i++)
            {
                int cnt = 0;
                for (int j = 0; j < Width; j++)
                {
                    if (_verIndexMap[i,j] == int.MaxValue)
                        _imageMatrix[i, cnt++] = OldImage[i, j];
                }
            }
            
        }
        #endregion
    }
}
