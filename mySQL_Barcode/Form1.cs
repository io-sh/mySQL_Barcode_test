using System.Windows.Forms;
using System;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ZXing;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;

namespace mySQL_Barcode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        VideoCapture video;
        Mat image;
        BarcodeReader reader;

        string company;
        string product;
        string production;
        DateTime date;
        string test;
        int year;
        int month;

        DBConnect db = new DBConnect();

        private void Form1_Load(object sender, EventArgs e)
        {
            video = new VideoCapture(0);//첫카메라
            image = new Mat();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            video.Read(image);
            pictureBox1.Image = image.ToBitmap();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string decoded = "";
            pictureBox2.Image = pictureBox1.Image;//화면 출력
            BarcodeReader reader = new BarcodeReader();//인스턴스 생성
            Result result = reader.Decode((Bitmap)pictureBox1.Image);//바코드 해석
            db.connect();


            // 결과를 확인합니다.
            if (result != null)
            {
                int[] num = { 2, 4, 2, 4, 1 };
                int currentPosition = 0;



                for (int i = 0; i < num.Length; i++)
                {
                    if (currentPosition + num[i] <= result.Text.Length)
                    {
                        string segment = result.Text.Substring(currentPosition, num[i]);
                        currentPosition += num[i];

                        if (i == 0)
                        {
                            switch (segment)
                            {
                                case "11":
                                    company = "삼성";
                                    break;
                                case "21":
                                    company = "엘지";
                                    break;
                                case "88":
                                    company = "씨제이";
                                    break;
                                case "99":
                                    company = "오뚜기";
                                    break;
                            }
                        }
                        else if (i == 1)
                        {
                            switch (segment)
                            {
                                case "1234":
                                    product = "세탁기";
                                    break;
                                case "1245":
                                    product = "냉장고";
                                    break;
                                case "1256":
                                    product = "정수기";
                                    break;
                                case "1267":
                                    product = "에어컨";
                                    break;
                                case "3412":
                                    product = "햇반";
                                    break;
                                case "3413":
                                    product = "밀가루";
                                    break;
                                case "3415":
                                    product = "만두";
                                    break;
                                case "3416":
                                    product = "라면";
                                    break;
                            }
                        }
                        else if (i == 2)
                        {
                            switch (segment)
                            {
                                case "11":
                                    production = "한국";
                                    break;
                                case "12":
                                    production = "차이나";
                                    break;
                                case "13":
                                    production = "베트남";
                                    break;
                                case "14":
                                    production = "태국";
                                    break;
                            }
                        }
                        else if (i == 3)
                        {
                            string yearSub = segment.Substring(0, 2);
                            string monthSub = segment.Substring(2, 2);
                            year = 2000 + int.Parse(yearSub);
                            month = int.Parse(monthSub);

                            date = new DateTime(year, month, 1);
                        }
                        else
                        {
                            test = segment;
                            // 바코드를 인식하지 못한 경우의 처리
                        }
                    }
                }
                //해석된 텍스트, 바코드 형식
                decoded = result.ToString() + "\r\n" + result.BarcodeFormat.ToString()
                     + $"\r\n {company}\r\n {product}\r\n {production}\r\n {date}";
                label1.Text = decoded;
                db.insert(company, product, production, date);

                dataGridView1.DataSource = db.dt;
            }
            else
            {
                label1.Text = "바코드를 인식하지 못하였습니다.";
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            db.select(name);
            dataGridView1.DataSource = db.dt;
        }
    }
}
