using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using static QRCoder.PayloadGenerator;

namespace QR_coder
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //klar gør QRCoder
            EncoderParameters myEncoderParameters;
            myEncoderParameters = new EncoderParameters(1);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            //Input tekts som laver det om det en QR senere
            Console.WriteLine("Insert link");
            string link = Console.ReadLine();

            // Laver QR ud fra teksten
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Gemmer QR-Koden i .exe mappen, og burde vises i programmet (dette er lavet i console.
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            link = link + ".jpg";
            qrCodeImage.Save(@link);
        }
    }
}
