using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spire.Doc;
using System.IO;
using System.Text;

namespace TrafficWaveService.Reports
{
    public class Converter
    {
        string _filePaht;
        string _fileName;
       public Converter(string pFilePaht, string pFileName)
        {
            _filePaht = pFilePaht;
            _fileName = pFileName;
        }
        public byte[] ConvertToPdf()
        {
            Document doc = new Document();
            doc.LoadFromFile(_filePaht + _fileName);
            doc.SaveToFile(_filePaht + "output.pdf", FileFormat.PDF);
            return getBytes(_filePaht + "output.pdf");
            
        }

        public byte[] ConvertToDocx()
        {
            return getBytes(_filePaht + _fileName);

        }

        /// <summary>
        /// Возврат байтов файла
        /// </summary>
        /// <param name="filePath">путь к файлу</param>
        /// <returns>байты файла</returns>
        public static byte[] getBytes(string filePath)
        {
            MemoryStream OutStream = new MemoryStream();
            byte[] outputBytes = new byte[1];
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    fs.CopyTo(OutStream);
                    BinaryReader reader = null;
                    outputBytes = new byte[(int)OutStream.Length];
                    reader = new BinaryReader(OutStream, Encoding.Unicode);
                    reader.BaseStream.Position = 0;
                    outputBytes = reader.ReadBytes((int)OutStream.Length);
                    reader.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {

            }
            OutStream.Close();
            return outputBytes;
        }
    }
}