using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;

namespace QR_coder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<qr_code> qr_list = new();
            Console.WriteLine("pls select what you want to do");

            Console.WriteLine("1. Save qr code to disk from server");
            Console.WriteLine("2. Save qr code to server from URL");
            string select = Console.ReadLine();


            if (int.Parse(select) == 1)
            {
                string conString = "Server=10.56.8.36;Database=DB81;User Id=STUDENT81;Password=OPENDB_81;";

                
                using (SqlConnection connection = new(conString))
                {

                    Console.Clear();
                    connection.Open();
                    string table = "Testimg";
                    string values = "Testimg.TestImgID, Testimg.imgName, Testimg.img ";
                    string CommandText = $"SELECT {values} FROM {table}";

                    SqlCommand sqlCommand = new(CommandText, connection);
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        
                        while (reader.Read() != false)
                        {
                            int imgID = int.Parse(reader["TestImgID"].ToString());
                            string imgName = reader["imgName"].ToString();
                            byte[] imgbyte = (byte[])reader["img"]; // Gets byte[] from server

                            qr_list.Add(new qr_code(imgID, imgName, imgbyte));//saves everything in a constructor

                        }
                    }
                    
                }
                Console.WriteLine("List over img in database");
                int x = 1;
                foreach (qr_code code in qr_list) // make a list over all qr-codes in sql
                {
                    Console.WriteLine(x + ". " + code.ImgName);
                    x++;
                }
                int slected_id = -1 + int.Parse(Console.ReadLine()); // pick by ID
                QRCodeGenerator qrGenerator = new QRCodeGenerator(); // Gets ready to genorate qr

                QRCodeData qrCodeData = new QRCodeData(qr_list[slected_id].Img, QRCodeData.Compression.Uncompressed); // de-compresser.
                
                QRCode qrCode = new QRCode(qrCodeData); // create qr-code from the data, all in memory
                Bitmap qrCodeImage = qrCode.GetGraphic(20); // draws img
                string fileName = qr_list[slected_id].ImgName; // file name
                qrCodeImage.Save(@fileName, System.Drawing.Imaging.ImageFormat.Jpeg); // saves img with location
                
                
            }
            else if (int.Parse(select) == 2)
            {

                Console.Clear();
                string filename = "newqr";

                //klar gør QRCoder
                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                //Input tekts som laver det om det en QR senere
                Console.WriteLine("Insert link");
                string link = Console.ReadLine();

                // Laver QR ud fra teksten
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.M);

                byte[] pic = qrCodeData.GetRawData(QRCodeData.Compression.Uncompressed); //compresses to a byte[]
                QRCode qrCode = new QRCode(qrCodeData);

                // Gemmer QR-Koden i .exe mappen, og burde vises i programmet (dette er lavet i console)
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                filename = filename + ".jpg";
                qrCodeImage.Save(@filename);

                string conString = "Server=10.56.8.36;Database=DB81;User Id=STUDENT81;Password=OPENDB_81;";
                SqlConnection conn = new SqlConnection(conString);


                try
                {

                    string sql = $"INSERT into Testimg (Testimg.imgName, Testimg.img) values('{link}', @Pic)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    
                    MemoryStream stream = new MemoryStream(); //åbner for memory
                    qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg); //bruges til at gemme billed fra, til memory

                    SqlParameter sqlParam = cmd.Parameters.AddWithValue("@Pic", pic);// erstatter @Pic med pic i sql sætningen
                    sqlParam.DbType = DbType.Binary; //converts byte[] to a varbinary()

                    conn.Open();
                    cmd.ExecuteNonQuery(); // executer sql
                    conn.Close();
                }
                finally
                {

                    conn.Close();

                }
            }
            else
            {
                Console.WriteLine("an error has been decteted, pls shut down the program and start again");
            }
        }
    }
    public class qr_code
    {
        private int Id;

        public int ID
        {
            get { return Id; }
            set { Id = value; }
        }
        private string imgName;

        public string ImgName
        {
            get { return imgName; }
            set { imgName = value; }
        }
        private byte[] img;

        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }

        public qr_code(int iD, string imgName, byte[] img)
        {
            ID = iD;
            ImgName = imgName;
            Img = img;
        }
    }
}
