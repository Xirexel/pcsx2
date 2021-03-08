using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCSXEmul.Tools
{
    class BiosControl
    {
        public static string FilePath { get; set; }
        
        static private void showErrorEvent(string a_message)
        {
        }

        // Loads the configured bios rom file into PS2 memory.  PS2 memory must be allocated prior to
        // this method being called.
        //
        // Remarks:
        //   This function does not fail if rom1, rom2, or erom files are missing, since none are
        //   explicitly required for most emulation tasks.
        //
        // Exceptions:
        //   BadStream - Thrown if the primary bios file (usually .bin) is not found, corrupted, etc.
        //
        static public bool LoadBIOS(IntPtr a_FirstArg, Int32 a_SecondArg)
        {
            bool l_result = false;

            try
            {
                do
                {                    
                    if (!File.Exists(FilePath))
                    {
                        var l_splitsFilePath = FilePath.Split(new char[] { '|' });

                        if (l_splitsFilePath == null || l_splitsFilePath.Length != 2)
                            break;

                        if (!File.Exists(l_splitsFilePath[0]))
                            break;

                        try
                        {
                            using (ArchiveFile archive = new ArchiveFile(l_splitsFilePath[0]))
                            {
                                var l_entry = archive.Entries.FirstOrDefault(p => p.FileName == l_splitsFilePath[1]);

                                if (l_entry != null)
                                {
                                    using (MemoryStream l_memoryStream = new MemoryStream())
                                    {
                                        try
                                        {
                                            l_entry.Extract(l_memoryStream);

                                            l_memoryStream.Position = 0;

                                            byte[] l_memory = l_memoryStream.ToArray();

                                            Marshal.Copy(l_memory, 0, a_FirstArg, Math.Min(a_SecondArg, l_memory.Length));
                                        }
                                        catch (Exception exc)
                                        {
                                            showErrorEvent(exc.Message);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            showErrorEvent(exc.Message);
                        }
                    }
                    else
                    {
                        var filesize = new System.IO.FileInfo(FilePath).Length;

                        if (filesize <= 0)
                        {
                            throw new FileNotFoundException();
                        }

                        using (var l_FileStream = File.Open(FilePath, FileMode.Open))
                        {
                            if (l_FileStream == null)
                                break;

                            byte[] l_memory = new byte[l_FileStream.Length];

                            l_FileStream.Read(l_memory, 0, l_memory.Length);

                            Marshal.Copy(l_memory, 0, a_FirstArg, Math.Min(a_SecondArg, l_memory.Length));
                        }
                    }

                    l_result = true;

                } while (false);



                //pxInputStream memfp(Bios, new wxMemoryInputStream(aPtrROM, aLength));
                //LoadBiosVersion( memfp, BiosVersion, BiosDescription, biosZone );

                //Console.SetTitle( pxsFmt( L"Running BIOS (%s v%u.%u)",
                //    WX_STR(biosZone), BiosVersion >> 8, BiosVersion & 0xff
                //));

                ////injectIRX("host.irx");	//not fully tested; still buggy

                ////LoadExtraRom( L"rom1", eeMem->ROM1 );
                ////LoadExtraRom( L"rom2", eeMem->ROM2 );
                ////LoadExtraRom( L"erom", eeMem->EROM );

                //if (g_Conf->CurrentIRX.Length() > 3)
                //    LoadIrx(g_Conf->CurrentIRX, &aPtrROM[0x3C0000]);

                //CurrentBiosInformation = NULL;
                //for (size_t i = 0; i < sizeof(biosVersions)/sizeof(biosVersions[0]); i++)
                //{
                //    if (biosVersions[i].biosChecksum == BiosChecksum && biosVersions[i].biosVersion == BiosVersion)
                //    {
                //        CurrentBiosInformation = &biosVersions[i];
                //        break;
                //    }
                //}
            }
            catch (Exception)
            {
                //// Rethrow as a Bios Load Failure, so that the user interface handling the exceptions
                //// can respond to it appropriately.
                //throw Exception::BiosLoadFailed( ex.StreamName )
                //    .SetDiagMsg( ex.DiagMsg() )
                //    .SetUserMsg( ex.UserMsg() );
            }

            return l_result;
        }
    }
}
