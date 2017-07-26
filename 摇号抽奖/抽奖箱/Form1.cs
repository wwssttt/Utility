using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 抽奖箱
{
    public partial class mainForm : Form
    {

        Config cfg = new Config();//参数配置
        Timer timer = new Timer();//定时器
        ArrayList labels = new ArrayList();//标签
        ArrayList lucky = new ArrayList();//已中奖


        public mainForm()
        {
            InitializeComponent();

            //全屏窗体并遮盖任务栏
            //必须放在initializecomponent之后，如果放在form_load里面则达不到效果
            this.FormBorderStyle = FormBorderStyle.None;//设置窗体无边框和标题栏
            //1.this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;//如果此处不注释则不会遮盖任务栏
            //如下两句如果不注释也会不遮盖任务栏
            //2.1.Screen s = Screen.PrimaryScreen;//获取主显示器
            //2.2.mainForm.ActiveForm.Size = new Size(s.Bounds.Width, s.Bounds.Height);//设置窗体的尺寸为主显示器尺寸
            this.WindowState = FormWindowState.Maximized;//设置窗体最大化
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //如果在此处加载mainform中的窗体全屏代码，则会出现任务栏不遮盖，且窗体下方和右方出现留白的情况

            Screen s = Screen.PrimaryScreen;//获取主显示器
            //mainForm.ActiveForm.Size = new Size(s.Bounds.Width, s.Bounds.Height);//设置窗体的尺寸为主显示器尺寸

            bool bgflag = File.Exists(Application.StartupPath + @"\bg.jpg");
            if(bgflag == true)
            {
                //mainForm.ActiveForm.BackgroundImage = Image.FromFile(Application.StartupPath + @"\bg.jpg");//设置背景图片
                //mainForm.ActiveForm.BackgroundImageLayout = ImageLayout.Stretch;
                Bitmap bm = new Bitmap(Application.StartupPath + @"\bg.jpg"); //fbImage图片路径
                this.BackgroundImage = bm;//设置背景图片
                this.BackgroundImageLayout = ImageLayout.Stretch;//设置背景图片自动适应
            }
            
            closeBtn.FlatStyle = FlatStyle.Flat;//样式  
            closeBtn.ForeColor = Color.Transparent;//前景  
            closeBtn.BackColor = Color.Transparent;//去背景  
            closeBtn.FlatAppearance.BorderSize = 0;//去边线  
            closeBtn.FlatAppearance.MouseOverBackColor = Color.Transparent;//鼠标经过  
            closeBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;//鼠标按下
            
            //开始时，结束按钮禁用
            endBtn.Enabled = false;
            //读取配置文件，看看生成多少中奖结果
            //动态生成中奖标签并赋值
            /*
            Label l1 = new Label();
            l1.Text = "0123456789";//内容
            l1.TextAlign = ContentAlignment.MiddleCenter;//居中
            l1.Width = (s.Bounds.Width-80) / 3;//宽度
            l1.Height = 50;//高度
            l1.Left = 10;//左侧距离
            l1.Top = 300;//顶部距离
            //l1.BackColor = Color.Transparent;//透明背景
            l1.ForeColor = Color.White;
            l1.Font = new Font("黑体", 26, l1.Font.Style | FontStyle.Regular);//设置字体，Bold,Italic
            mainForm.ActiveForm.Controls.Add(l1);//添加控件
            */
            int outer = 50;
            int inner = 30;
            int top = 200;
            int width = (s.Bounds.Width - 2 * outer - (cfg.numberofLine - 1) * inner) / cfg.numberofLine;
            int height = 100;
            int hdis = 100;
            for(int i = 0; i < cfg.number; i++)
            {
                Label l = new Label();
                l.Text = "幸运观众抽奖";//内容
                l.TextAlign = ContentAlignment.MiddleCenter;//居中
                l.Width = width;//宽度
                l.Height = height;//高度
                l.Top = top + hdis * (i / cfg.numberofLine);//每列的高度
                l.Left = outer + (i % cfg.numberofLine) * (width + inner);
                l.BackColor = Color.Transparent;//透明背景
                l.ForeColor = Color.White;
                //MessageBox.Show(cfg.fontname.ToString(), "系统提示4");
                l.Font = new Font(cfg.fontname, 56, l.Font.Style | FontStyle.Regular);//设置字体，Bold,Italic
                labels.Add(l);
                mainForm.ActiveForm.Controls.Add(l);//添加控件
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            //点击开始后的操作
            //1.结束按钮禁用，开始启用
            endBtn.Enabled = false;
            startBtn.Enabled = true;
            //2.读取配置，生成摇号结果
            //遍历所有Label控件
            //这里使用数组存储之前建立的标签
            //如果使用mainform.controls进行遍历，则如果窗口运行时再运行其他应用程序则会报错
            foreach (Label labelcon in labels)
            {
                if (labelcon.GetType().ToString() == "System.Windows.Forms.Label")
                {
                    //还有号可摇
                    if(cfg.curindex < cfg.data.Count)
                    {
                        string luckyname = labelcon.Text;
                        while (lucky.Contains(luckyname))
                        {
                            Random num = new Random();
                            int index = num.Next(0, cfg.data.Count - 1);
                            luckyname = cfg.data[index].ToString();
                        }
                        lucky.Add(luckyname);
                        labelcon.Text = luckyname;
                        //labelcon.Text = cfg.data[cfg.curindex].ToString();
                        //cfg.curindex++;
                        
                    }
                    else
                    {
                        labelcon.Text = "";//数字显示完，清空剩余标签内容
                    }
                }
            }
            //是否显示中奖信息
            if(cfg.levelflag == "on")
            {
                MessageBox.Show(cfg.level, cfg.leveltitle);
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            //还有号可摇
            if (cfg.curindex < cfg.data.Count)
            {
                //点击开始后的操作
                //1.开始按钮禁用，结束按钮启用
                startBtn.Enabled = false;
                startBtn.Text = "再摇一次";
                endBtn.Enabled = true;
                //2.读取配置开始摇号
                timer.Interval = 30;//100 = 0.1秒
                timer.Tick += new EventHandler(timer_Tick);//绑定间隔事件
                timer.Enabled = true;//开启定时器
            }
            else
            {
                MessageBox.Show("已经没有更多的摇号机会，请退出", "系统提醒");
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            foreach (Label labelcon in labels)
            {
                if (labelcon.GetType().ToString() == "System.Windows.Forms.Label")
                {
                    Random num = new Random();
                    int index = num.Next(0, cfg.data.Count - 1);
                    labelcon.Text = cfg.data[index].ToString();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确定要关闭抽奖箱程序吗？", "重要提醒", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                //用户选择确认的操作
                System.Environment.Exit(0);
            }
        }
    }

    //读取配置文件类
    public class Config
    {
        public int curindex = 0;//当前抽奖索引
        public int number = 0;//抽奖数
        public ArrayList data = new ArrayList();//抽奖数字
        public string levelflag = "off";//等级开关
        public string leveltitle = "中奖提醒";//等级标题
        public string level = "";//抽奖等级
        public string fontname = "黑体";//数字字体
        public int numberofLine = 3;//每列数字数量
        
        public Config()
        {
            bool hasCfgFile = File.Exists(Application.StartupPath + @"\cfg.ini");
            if (hasCfgFile == false)
            {
                MessageBox.Show("未找到配置文件cfg.ini，请在应用程序同级文件夹新建并配置", "系统提示");
                System.Environment.Exit(0);
            }
            StreamReader reader = new StreamReader(Application.StartupPath + @"\cfg.ini", Encoding.Default);
            string flag = "";
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Equals("抽奖数") == true)
                {
                    flag = "number";
                } else if (line.Equals("抽奖号码") == true)
                {
                    flag = "data";
                } else if (line.Equals("等级开关") == true)
                {
                    flag = "levelflag";
                }
                else if (line.Equals("等级标题") == true)
                {
                    flag = "leveltitle";
                }
                else if (line.Equals("等级") == true)
                {
                    flag = "level";
                }else if (line.Equals("字体") == true)
                {
                    flag = "font";
                }
                else if (line.Equals("每列数量") == true)
                {
                    flag = "numberofline";
                }
                else
                {
                    if(flag == "number")
                    {
                        number = int.Parse(line);
                    }else if (flag == "data")
                    {
                        data.Add(line);
                    }else if (flag == "levelflag")
                    {
                        if(line == "on")
                        {
                            levelflag = "on";
                        }
                    }else if (flag == "leveltitle")
                    {
                        leveltitle = line;
                    }
                    else if (flag == "level")
                    {
                        level = line;
                    }else if (flag == "font")
                    {
                        fontname = line;
                    }
                    else if (flag == "numberofline")
                    {
                        numberofLine = int.Parse(line);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            reader.Close();
        }
    }
}
