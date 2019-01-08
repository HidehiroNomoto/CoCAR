///////////////////////////////////////////////////////////////////////////////
//
// (c) 2004 J. A. Robson, http://www.arbingersys.com
//
//
// Description: Test madxlib from C#
//
// Notes:
//
// .NET CLR has a problem with arrays (at least the multi-dimensional
// kind) in structures. The "madx_house" struct from "madxlib.h" 
// references structures inherent to the operation of libmad, such as
// "mad_stream" and "mad_frame". Some of the these structures have
// multi-dimensioned arrays as members. C# croaked on this, so I 
// got around it by determining the total size in bytes of 
// "madx_house", and creating a byte array of that very size. I then
// used "GCHandle.Alloc()" to get a pointer to it, and passed 
// this pointer to the madx_* functions, which were none the wiser.
//
// It does create a problem of referencing the data in the said 
// structures, but for the code below it was not an issue.
//
//
///////////////////////////////////////////////////////////////////////////////


namespace MadxTest
{

	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	

	class Test
	{
	

		public const int MADX_OUTPUT_BUFFER_SIZE = (1152*8);
		public const int MADX_INPUT_BUFFER_SIZE	 = (5*1152*8);

		
		// Gotten from sizeof(madx_house):
		public const int MADX_HOUSE_SIZE = 22672; 
		

		
		// The easy one
		public enum madx_sig : int
		{
			ERROR_OCCURED,
			MORE_INPUT,
			FLUSH_BUFFER,
			EOF_REACHED,
			CALL_AGAIN
		}

	

		[StructLayout(LayoutKind.Sequential, Size=276), Serializable]
	  	public struct madx_stat
	  	{
				
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
			public string	msg;
			public uint 	write_size;
			public int		is_eof;
			public uint		readsize;
			public uint		remaining;
			public IntPtr	buffstart;
			
		}
		


		
		

		[DllImport("madxlib.dll")]
		public static extern int
		madx_init ( IntPtr out_buffer, IntPtr mx_house );	



		[DllImport("madxlib.dll")]
		public static extern madx_sig
		madx_read ( IntPtr in_buffer,	
			IntPtr out_buffer, IntPtr mx_house, ref madx_stat mxstat );	

		
		
		[DllImport("madxlib.dll")]
		public static extern void
		madx_deinit( IntPtr mx_house );		




		

			
		static void 
		Main(string[] args) 
		{

				
			madx_sig	mxSignal;
			madx_stat 	mxStat = new madx_stat();
			string		outputFile = "TestCSharp.pcm";			




			// Test arguments

			if (args.Length == 0) 
			{
				Console.WriteLine("usage: TestCSharp 'file.mp3'");
				return;
			}


			// set the input file	

			string inputFile = args[0];

			

			
			// Trickery to get around limitation in .NET
			// with structs and arrays (couldn't map 
			// madx_house struct directly).

			byte[] mxHouse = new byte[MADX_HOUSE_SIZE]; 
			GCHandle handle = GCHandle.Alloc( mxHouse, GCHandleType.Pinned ); 
			IntPtr mxHousePtr = handle.AddrOfPinnedObject(); 
			

			
			// Pin output buffer, assign to pointer

			byte[] outBuffer = new byte[MADX_OUTPUT_BUFFER_SIZE];
			GCHandle handleA = 
				GCHandle.Alloc( outBuffer, GCHandleType.Pinned ); 
			IntPtr outBufferPtr = handleA.AddrOfPinnedObject(); 
			

			
			// Pin input buffer, assign to pointer

			byte[] inBuffer = new byte[MADX_INPUT_BUFFER_SIZE];
			GCHandle handleB = 
				GCHandle.Alloc( inBuffer, GCHandleType.Pinned ); 
			IntPtr inBufferPtr = handleB.AddrOfPinnedObject(); 

			


			

			
			// Initialize madxlib
			
			if ( (madx_init(outBufferPtr, mxHousePtr)) == 0 )
			{
				Console.WriteLine("Error opening " + args[0]);
				return;
			}

			

			

			// open inputFile as binary

			Stream s = 
				new FileStream(inputFile, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(s);
			
			

			// Open output file as binary

			Stream os = new FileStream(outputFile, FileMode.Create);
			BinaryWriter bw = new BinaryWriter(os); 
			
		


			

			// Process input, save output

			do
			{


				mxSignal = madx_read(
					inBufferPtr, outBufferPtr, mxHousePtr, ref mxStat);
				

				if (mxStat.msg != "") Console.WriteLine("{0}", mxStat.msg);
				


				
				if (mxSignal == madx_sig.ERROR_OCCURED)			// Error
				{


					Console.WriteLine("Unrecoverable error {0}", mxStat.msg);
					break;

					
				}
				else if (mxSignal == madx_sig.MORE_INPUT)		// Input
				{

					

					// Partial read
										
					if ((int)mxStat.buffstart != 0)
					{
					
						int a;
						if ( (a = br.Read(inBuffer, (int)(MADX_INPUT_BUFFER_SIZE-mxStat.readsize), (int)mxStat.readsize )) == (int)mxStat.readsize )
						{
							Console.WriteLine("Partial buffer {0}", a);
						}
						else
						{
							if (s.Position == s.Length) mxStat.is_eof = 1;
							mxStat.readsize = (uint)a;
						}
					
						
					}
					else		// Full read
					{
						
						int a;
						if ( (a = br.Read(inBuffer, 0, MADX_INPUT_BUFFER_SIZE )) == (int)mxStat.readsize )
						{
							Console.WriteLine("Full buffer {0}", a);
						}
						else
						{
							if (s.Position == s.Length) mxStat.is_eof = 1;
							mxStat.readsize = (uint)a;
						}

						
					}
						

					

				}
				else if (mxSignal == madx_sig.FLUSH_BUFFER) 	// Write
				{
					
					
					bw.Write( outBuffer, 0, (int)mxStat.write_size );
					Console.WriteLine("Buffer written");
					

				}
				else if (mxSignal == madx_sig.EOF_REACHED) 		// End
				{ 
				
					
					bw.Write( outBuffer,0, (int)mxStat.write_size );
					Console.WriteLine("Finished. {0}",(int)mxStat.write_size);
					break; 
					
					
				}

				

			} while(true);
		


			handle.Free(); 
			handleA.Free(); 


			s.Close();
			os.Close();


		} // Main()



			
	} // class Test
		

	
} // namespace MadxTest


