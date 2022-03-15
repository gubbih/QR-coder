using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace QR_coder
{
    internal class Program
    {
        static void Main(string[] args)
        {            
            QRCodeGenerator qrGenerator = new(); //Gets QRcoder ready
            string conString = "Server=10.56.8.36;Database=DB81;User Id=STUDENT81;Password=OPENDB_81;";
            List<Qr_code> qr_list = new();


            Console.WriteLine("pls select what you want to do");

            Console.WriteLine("1. Save qr code to disk from server");
            Console.WriteLine("2. Save qr code to server from URL");
            string select = Console.ReadLine();
            Console.Clear();

            if (int.Parse(select) == 1)
            {
                using (SqlConnection connection = new(conString))
                {

                    connection.Open();
                    string table = "Testimg";
                    string values = "Testimg.TestImgID, Testimg.imgName, Testimg.img ";
                    string CommandText = $"SELECT {values} FROM {table}";

                    SqlCommand sqlCommand = new(CommandText, connection);
                    using SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read() != false)
                    {
                        int imgID = int.Parse(reader["TestImgID"].ToString());//hej :)
                        string imgName = reader["imgName"].ToString();
                        byte[] imgbyte = (byte[])reader["img"]; // Gets byte[] from server

                        qr_list.Add(new Qr_code(imgID, imgName, imgbyte));//saves everything in a constructor

                    }
                    connection.Close();

                }
                Console.WriteLine("List over img in database");
                int x = 1;
                foreach (Qr_code code in qr_list) // make a list over all qr-codes in sql
                {
                    Console.WriteLine(x + ". " + code.ImgName);
                    x++;
                }
                int slected_id = -1 + int.Parse(Console.ReadLine()); // pick by ID

                QRCodeData qrCodeData = new QRCodeData(qr_list[slected_id].ImgByte, QRCodeData.Compression.Uncompressed); // de-compresser.
                

                QRCode qrCode = new QRCode(qrCodeData); // create qr-code from the data, all in memory
                Bitmap qrCodeImage = qrCode.GetGraphic(20); // draws img
                string fileName = qr_list[slected_id].ImgName + ".jpg"; // file name, must declare file extension
                qrCodeImage.Save(@fileName, System.Drawing.Imaging.ImageFormat.Jpeg); // saves img with location
                
                
            }
            else if (int.Parse(select) == 2)
            {            
                Console.WriteLine("Insert link");
                string link = Console.ReadLine();

                // Makes QR code out of a string
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.M);

                byte[] pic = qrCodeData.GetRawData(QRCodeData.Compression.Uncompressed); //compresses to a byte[]
                QRCode qrCode = new(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(20); // Saves qrcode in memory
                string FileName = link + ".jpg"; // must declare file extenstion
                qrCodeImage.Save(FileName, System.Drawing.Imaging.ImageFormat.Jpeg);//saves QR code in same folder as the exe is located

                SqlConnection conn = new(conString);


                try
                {

                    string sql = $"INSERT into Testimg (Testimg.imgName, Testimg.img) values('{link}', @Pic)";
                    SqlCommand cmd = new(sql, conn);

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
    public class Qr_code
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
        private byte[] imgByte;

        public byte[] ImgByte
        {
            get { return imgByte; }
            set { imgByte = value; }
        }

        public Qr_code(int iD, string imgName, byte[] imgByte)
        {
            ID = iD;
            ImgName = imgName;
            ImgByte = imgByte;
        }
    }
}
