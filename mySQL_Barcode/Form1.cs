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
            video = new VideoCapture(0);//ùī�޶�
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
            pictureBox2.Image = pictureBox1.Image;//ȭ�� ���
            BarcodeReader reader = new BarcodeReader();//�ν��Ͻ� ����
            Result result = reader.Decode((Bitmap)pictureBox1.Image);//���ڵ� �ؼ�
            db.connect();


            // ����� Ȯ���մϴ�.
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
                                    company = "�Ｚ";
                                    break;
                                case "21":
                                    company = "����";
                                    break;
                                case "88":
                                    company = "������";
                                    break;
                                case "99":
                                    company = "���ѱ�";
                                    break;
                            }
                        }
                        else if (i == 1)
                        {
                            switch (segment)
                            {
                                case "1234":
                                    product = "��Ź��";
                                    break;
                                case "1245":
                                    product = "�����";
                                    break;
                                case "1256":
                                    product = "������";
                                    break;
                                case "1267":
                                    product = "������";
                                    break;
                                case "3412":
                                    product = "�޹�";
                                    break;
                                case "3413":
                                    product = "�а���";
                                    break;
                                case "3415":
                                    product = "����";
                                    break;
                                case "3416":
                                    product = "���";
                                    break;
                            }
                        }
                        else if (i == 2)
                        {
                            switch (segment)
                            {
                                case "11":
                                    production = "�ѱ�";
                                    break;
                                case "12":
                                    production = "���̳�";
                                    break;
                                case "13":
                                    production = "��Ʈ��";
                                    break;
                                case "14":
                                    production = "�±�";
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
                            // ���ڵ带 �ν����� ���� ����� ó��
                        }
                    }
                }
                //�ؼ��� �ؽ�Ʈ, ���ڵ� ����
                decoded = result.ToString() + "\r\n" + result.BarcodeFormat.ToString()
                     + $"\r\n {company}\r\n {product}\r\n {production}\r\n {date}";
                label1.Text = decoded;
                db.insert(company, product, production, date);

                dataGridView1.DataSource = db.dt;
            }
            else
            {
                label1.Text = "���ڵ带 �ν����� ���Ͽ����ϴ�.";
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
