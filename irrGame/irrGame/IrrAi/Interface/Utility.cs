using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

namespace IrrGame.IrrAi.Interface
{
    public static class Utility
    {
        public static bool getColourFrom(string readBuffer, ref Color col)
        {
            try
            {
                if (col == null)
                    col = new Color();

                string[] aStr = readBuffer.Split(new char[] { ',' });

                if (aStr.Length == 4)
                {
                    col.Red = int.Parse(aStr[0]);
                    col.Green = int.Parse(aStr[1]);
                    col.Blue = int.Parse(aStr[2]);
                    col.Alpha = int.Parse(aStr[3]);
                }
                else
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
// 
//             if (readBuffer.Length==0 || bufferSize == 0) 
//                 return;
// 
// 	        byte[] pReadBuffer = readBuffer;
// 	        int[] colour= new int[4]{0,0,0,0};
// 	        int i = 0;
//   
//             int pReadBufferK = 0;
//             int readBufferK = 0;
// 	        // Parse out the colour, should be a comma delimted list of 4 integers
// 	        while (pReadBufferK-readBufferK < bufferSize && i < 4)
//             {
// 		        colour[i++] = BitConverter.ToInt32(pReadBuffer,pReadBufferK);
//                 i++;
// 
//                 if (i > 3) 
//                     break;
// 
// 		        while (pReadBuffer[pReadBufferK] != ',')
//                     ++pReadBufferK; // find the next comma
// 
// 		        ++pReadBufferK; // skip the comma      
// 	        }
//   
// 	        if (col!=null)
//                 col.Set(colour[0], colour[1], colour[2], colour[3]); 
        }


        
        public static bool getVector3dfFrom(string sBuffer, ref Vector3Df vec)
        {
            try
            {
                if (vec == null)
                    vec = new Vector3Df();

                string[] aStr = sBuffer.Split(new char[] { ',' });
                if (aStr.Length == 3)
                {
                    aStr[0] = aStr[0].Replace('.', ',');
                    aStr[1] = aStr[1].Replace('.', ',');
                    aStr[2] = aStr[2].Replace('.', ',');

                    vec.Set(float.Parse(aStr[0]), float.Parse(aStr[1]), float.Parse(aStr[2]));
                }
                else
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
//             if (readBuffer.Length==0 || bufferSize == 0)
//                 return;
//             
// 	        byte[] pReadBuffer = readBuffer;
// 	        float[] vector = new float[3]{0,0,0};
// 	        int i = 0;
// 
//             int pReadBufferK = 0;
//             int readBufferK = 0;
// 	        // Parse out the vector, should be a comma delimted list of 3 floats
// 	        while (pReadBufferK-readBufferK < bufferSize && i < 3)
//             {
// 		        vector[i++] = BitConverter.ToSingle(pReadBuffer,pReadBufferK);
//                 i++;
// 
// 		        if (i > 2)
//                     break;
// 
// 		        while (pReadBuffer[pReadBufferK] != ',')
//                     ++pReadBufferK; // find the next comma
// 
// 		        ++pReadBufferK; // skip the comma      
// 	        }
//   
// 	        if (vec!=null)
//                 vec.Set(vector[0],vector[1],vector[2]);
        }

        public static bool getDimension2DfFrom(string sBuffer, ref Dimension2Df dim)
        {
            try
            {
                if (dim == null)
                    dim = new Dimension2Df();

                string[] aStr = sBuffer.Split(new char[] { ',' });
                
                if (aStr.Length == 2)
                {
                    aStr[0] = aStr[0].Replace('.', ',');
                    aStr[1] = aStr[1].Replace('.', ',');


                    dim.Width = float.Parse(aStr[0]);
                    dim.Height = float.Parse(aStr[1]);
                }
                else
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            //             if (readBuffer.Length==0 || bufferSize == 0)
            //                 return;
            //             
            // 	        byte[] pReadBuffer = readBuffer;
            // 	        float[] vector = new float[3]{0,0,0};
            // 	        int i = 0;
            // 
            //             int pReadBufferK = 0;
            //             int readBufferK = 0;
            // 	        // Parse out the vector, should be a comma delimted list of 3 floats
            // 	        while (pReadBufferK-readBufferK < bufferSize && i < 3)
            //             {
            // 		        vector[i++] = BitConverter.ToSingle(pReadBuffer,pReadBufferK);
            //                 i++;
            // 
            // 		        if (i > 2)
            //                     break;
            // 
            // 		        while (pReadBuffer[pReadBufferK] != ',')
            //                     ++pReadBufferK; // find the next comma
            // 
            // 		        ++pReadBufferK; // skip the comma      
            // 	        }
            //   
            // 	        if (vec!=null)
            //                 vec.Set(vector[0],vector[1],vector[2]);
        }


//         
//         public static void getDimension2dfFrom(byte[] readBuffer, int bufferSize, Dimension2Df dim)
//         {
//             if (readBuffer.Length==0 || bufferSize == 0)
//                 return;
//             
//             byte[] pReadBuffer = readBuffer;
// 	        float[] dimension = new float[2]{0,0};
// 	        int i = 0;
// 
//             int pReadBufferK = 0;
//             int readBufferK = 0;
// 	        // Parse out the vector, should be a comma delimted list of 3 floats
// 	        while (pReadBufferK-readBufferK < bufferSize && i < 2)
//             {
// 		        dimension[i++] = BitConverter.ToSingle(pReadBuffer,pReadBufferK);
//                 i++;
// 
// 		        if (i > 1)
//                     break;
// 		
//                 while (pReadBuffer[pReadBufferK] != ',')
//                     ++pReadBufferK; // find the next comma
// 
// 		        ++pReadBufferK; // skip the comma      
// 	        }
//   
// 	        if (dim!=null)
//                 dim.Set(dimension[0],dimension[1]);
//         }

    }
}




