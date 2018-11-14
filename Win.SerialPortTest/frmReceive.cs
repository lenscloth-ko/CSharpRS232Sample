using System;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

/// <summary>
/// Virtual Serial Port Driver
/// https://www.eltima.com/products/vspdxp/
/// COMx / COMy pair
/// </summary>
namespace Win.SerialPortTest
{
    
    public partial class frmReceive : Form
    {
        /// <summary>
        /// SerialPort 인스턴스
        /// </summary>
        SerialPort sp = new SerialPort();
        /// <summary>
        /// data recived 이벤트 핸들러 안에서 비동기로 data를 전송할 대리자
        /// </summary>
        /// <param name="callbackText"></param>
        //delegate void GetCallBack(string callbackText);
        delegate void GetRecivedTextCallBack(string callbackText);

        /// <summary>
        /// 생성자
        /// </summary>
        public frmReceive()
        {
            InitializeComponent();

            SetPortName(comboBox_PortName); // Port확인 및 Bind
            button_Close.Enabled = false; // 해제버튼은 시작시에는 작동불가

            sp.DataReceived += new SerialDataReceivedEventHandler(serialDataReceived);
        }

        /// <summary>
        /// SerialPort DataReceived 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //리시브 사이즈, 버퍼..., data
            int received_size = sp.BytesToRead;
            byte[] temp_buffer = new byte[received_size];
            string received_data = string.Empty;

            sp.Read(temp_buffer, 0, received_size);
            received_data = Encoding.Default.GetString(temp_buffer);
            this.BeginInvoke(new GetRecivedTextCallBack(callbackText), new object[] { received_data });

            //throw new NotImplementedException();
        }

        /// <summary>
        /// callback 문자 수신
        /// </summary>
        /// <param name="callbackText"></param>
        private void callbackText(string callbackText)
        {
            AddLogText(string.Format("[수신] : {0}", callbackText));
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 선택 가능한 Port를 확인후 combobox bind
        /// </summary>
        /// <param name="cbx"></param>
        private void SetPortName(ComboBox cbx)
        {
            comboBox_PortName.Items.Clear();

            if (SerialPort.GetPortNames().Length <= 0)
            {
                //UI 상태 변경
                cbx.Enabled = false;
                button_Open.Enabled = false;

                AddLogText("선택 가능한 PORT가 없습니다.");
            }
            else
            {
                //UI 상태 변경
                cbx.Enabled = true;
                button_Open.Enabled = true;

                foreach (string portName in SerialPort.GetPortNames())
                {
                    comboBox_PortName.Items.Add(portName);
                    AddLogText(string.Format("{0} port 확인", portName));
                }
            }
        }

        /// <summary>
        /// 연결 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Open_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox_PortName.Text))
            {
                MessageBox.Show("Port를 선택해 주시기 바랍니다.");
                comboBox_PortName.Focus();
            }
            else
            {
                sp.PortName = comboBox_PortName.Text;
                if (string.IsNullOrEmpty(textBox_BaudRate.Text))
                {
                    sp.BaudRate = 115200;
                }
                else
                {
                    sp.BaudRate = Convert.ToInt32(textBox_BaudRate.Text);
                }

                if (!sp.IsOpen)
                {
                    try
                    {
                        sp.Open();
                        AddLogText(string.Format("{0} Port OPEN", comboBox_PortName.Text));

                        //UI 상태 변경
                        comboBox_PortName.Enabled = false;
                        button_Open.Enabled = false;
                        button_Close.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        AddLogText(ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 연결 해제 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Close_Click(object sender, EventArgs e)
        {
            if (sp.IsOpen)
            {
                sp.DiscardOutBuffer();
                sp.DiscardInBuffer();
                sp.Close();

                AddLogText(string.Format("{0} Port CLOSE", comboBox_PortName.Text));

                //UI 상태 변경
                comboBox_PortName.Enabled = true;
                button_Open.Enabled = true;
                button_Close.Enabled = false;
            }
        }

        /// <summary>
        /// Port목록 새로고침 이벤트 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Rescan_Click(object sender, EventArgs e)
        {
            SetPortName(comboBox_PortName); // Port확인 및 Bind
        }

        #region 공통
        /// <summary>
        /// 로그 텍스트 추가
        /// </summary>
        /// <param name="msg"></param>
        private void AddLogText(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                listBox_Log.Items.Add(msg);
            }
        }
        #endregion

        /// <summary>
        /// BaudRate 숫자만 입력받게
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_BaudRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }
    }
}
