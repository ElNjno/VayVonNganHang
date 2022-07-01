using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace VayVon_ID3
{
    public struct NodeChuaLa
    {
        public string name;
        public string value;
        public List<NodeChuaLa> arrNodes;
        public bool nodeRoot;
    };

    public partial class Form1 : Form
    {
        public List<List<string>> dtSource;
        DataTable dt = new DataTable();

        List<List<string>> dataSource; //Mảng các thuộc tính và giá trị thực
        List<string> arrClause; //Chứa các thuộc tính(Attributes)
        List<string> arrValue; //Chứa các giá trị hàm mục tiêu
        Dictionary<string, double> valueAttribute; // Information Gain của từng giá trị thuộc tính(Attribute)
        Dictionary<string, bool> boolValueAttribute; // trả về true nếu giá trị của thộc tính(Attribute) chỉ cho ra một giá trị thuộc tính kết quả và ngược lại.
        private Graphics grs;
        NodeChuaLa decisionTree;


        private void DrawDecisionTree(NodeChuaLa decisionTreeX, Graphics grs, PointF point, bool start, float kc)
        {
            try
            {
                float tempX = point.X - 20;
                float tempY = point.Y + 15;
                if (start)
                {
                    tempX = point.X / 2;
                    tempY = point.Y + 15;
                }
                grs.DrawString(decisionTreeX.name, new Font("Segoe UI", 12), new SolidBrush(Color.Yellow), tempX - decisionTreeX.name.Length * 2, point.Y);
                int sumNode = decisionTreeX.arrNodes.Count();
                float width = point.X / (sumNode + 1);
                int ss = sumNode / 2 + 1;
                float tmpX;
                kc /= sumNode;
                for (int i = 1; i <= sumNode; i++)
                {
                    if (i < ss)
                    {
                        tmpX = tempX - kc * (i / ss + 1);
                    }
                    else
                    {
                        if (i == ss)
                        {
                            if (i == sumNode) tmpX = tempX + kc * (i / ss + 1);
                            else
                                tmpX = tempX;
                        }
                        else
                            tmpX = tempX + kc * (i / ss + 1);
                    }
                    if (start)
                    {
                        grs.DrawLine(new Pen(Color.Black), tempX + 20, tempY + 5, width * i + 20, tempY + 50);
                        grs.DrawString(decisionTreeX.arrNodes[i - 1].name, new Font("Segoe UI", 12), new SolidBrush(Color.DarkGreen), width * i - 10, tempY + 55);
                        if (decisionTreeX.arrNodes[i - 1].arrNodes == null)
                        {
                            if (decisionTreeX.arrNodes[i - 1].value != null)
                            {
                                grs.DrawLine(new Pen(Color.Black), width * i + 20, tempY + 75, width * i + 20, tempY + 110);
                                grs.DrawString(decisionTreeX.arrNodes[i - 1].value, new Font("Segoe UI", 12), new SolidBrush(Color.DarkBlue), width * i + 10, tempY + 110);
                            }
                        }
                        else
                        {
                            ss = sumNode / 2 + 1;
                            if (i < ss)
                            {
                                tmpX = width * i - 50 * (i / ss);
                            }
                            else
                                tmpX = width * i + 50 * (i / ss);
                            grs.DrawLine(new Pen(Color.Black), width * i + 20, tempY + 80, tmpX, tempY + 110);
                            //adasd
                            DrawDecisionTree(decisionTreeX.arrNodes[i - 1].arrNodes[0], grs, new PointF(tmpX, tempY + 110), false, kc);
                        }
                    }
                    else
                    {
                        grs.DrawLine(new Pen(Color.Black), tempX + 20, tempY + 5, tmpX, tempY + 50);
                        grs.DrawString(decisionTreeX.arrNodes[i - 1].name, new Font("Segoe UI", 12), new SolidBrush(Color.DarkGreen), tmpX - 20, tempY + 55);
                        if (decisionTreeX.arrNodes[i - 1].arrNodes == null)
                        {
                            if (decisionTreeX.arrNodes[i - 1].value != null)
                            {
                                grs.DrawLine(new Pen(Color.Black), tmpX, tempY + 80, tmpX, tempY + 110);
                                grs.DrawString(decisionTreeX.arrNodes[i - 1].value, new Font("Segoe UI", 12), new SolidBrush(Color.DarkBlue), tmpX - 10, tempY + 110);
                            }
                        }
                        else
                        {
                            ss = sumNode / 2 + 1;
                            if (i < ss)
                            {
                                tmpX = width * i - 50 * (i / ss);
                            }
                            else
                                tmpX = width * i + 50 * (i / ss);
                            grs.DrawLine(new Pen(Color.Black), width * i + 20, tempY + 80, tmpX, tempY + 110);
                            DrawDecisionTree(decisionTreeX.arrNodes[i - 1].arrNodes[0], grs, new PointF(tmpX, tempY + 110), false, kc);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("dữ liệu lỗi");
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenTextSource_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Open data source";
                ofd.Filter = "File text|*.txt|All file|*.*";
                List<List<string>> arrObject = new List<List<string>>();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    StreamReader sr = new StreamReader(ofd.FileName, Encoding.UTF8);
                    string line = "";
                    int index = 0;
                    DataTable dt = new DataTable();
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] obj = line.Trim().Split(' ');
                        arrObject.Add(new List<string>());
                        arrObject[index++] = obj.ToList();
                    }

                    for (int i = 0; i < arrObject.Count(); i++)
                    {
                        if (i == 0)
                        {
                            foreach (string item in arrObject[0])
                            {
                                dt.Columns.Add(item);
                            }
                        }
                        else
                        {
                            dt.Rows.Add(arrObject[i].ToArray());
                        }
                    }
                    dataGridViewClauses.DataSource = dt;
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load error: " + ex.Message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Kiểm tra xem node đó có phải node lá hay ko
        private bool checkNodeLa(Dictionary<string, bool> boolValueAttribute, string name)
        {
            if (boolValueAttribute[name])
                return true;
            return false;
        }

        //Trả về I (độ lợi thông tin của nút chủ (nút từ đó chọn nút kế tiếp))
        private double TinhGain(List<List<string>> data)
        {
            int s1 = 0, s2 = 0;
            double infoGain = 0f;
            int numRow = data.Count();

            for (int i = 1; i < data[numRow - 1].Count(); i++)
            {
                if (arrValue[0] == data[numRow - 1][i])
                    s1++;
                else
                    s2++;
            }

            infoGain = -(1.0 * s1 / (s1 + s2)) * Math.Log(1.0 * s1 / (s1 + s2), 2f) - (1.0 * s2 / (s1 + s2)) * Math.Log(1.0 * s2 / (s1 + s2), 2f);
            return infoGain;
        }


        //Trả về Entropy của giá trị (name) trong thuộc tính(Attribute) vị trí column
        private double tinhEntropy(List<List<string>> data, ref Dictionary<string, bool> boolValueAttribute, int column, string name)
        {
            double entropy = 0f;
            int s1 = 0, s2 = 0;
            int numRow = data.Count();
            int numColumn = data[numRow - 1].Count();
            for (int i = 1; i < numColumn; i++)
            {
                if (data[column][i] == name)
                {
                    if (data[numRow - 1][i] == arrValue[0])
                        s1++;
                    else
                        s2++;
                }
            }

            if (s1 == 0 || s2 == 0)
            {
                if (boolValueAttribute.ContainsKey(data[column][0] + name))
                {
                    boolValueAttribute[data[column][0] + name] = true;
                }
                else
                    boolValueAttribute.Add(data[column][0] + name, true);
                return 0f;
            }
            else
            {
                if (boolValueAttribute.ContainsKey(data[column][0] + name))
                {
                    boolValueAttribute[data[column][0] + name] = false;
                }
                else
                    boolValueAttribute.Add(data[column][0] + name, false);
            }

            entropy = (1f * (s1 + s2) / (numColumn - 1)) * tinhGainThuocTinh(s1, s2);
            return entropy;
        }

        //Trả về I (độ lợi thông tin của từng giá trị trong mỗi thuộc tính(Attribute))
        private double tinhGainThuocTinh(int s1, int s2)
        {
            double infoGain = 0f;
            infoGain = -(1.0 * s1 / (s1 + s2)) * Math.Log(1.0 * s1 / (s1 + s2), 2f) - (1.0 * s2 / (s1 + s2)) * Math.Log(1.0 * s2 / (s1 + s2), 2f);
            return infoGain;
        }

        //Trả về Gain của thuộc tính(Attribute) vị trí column
        private double tinhGiaTriGain(string node, List<List<string>> data, ref Dictionary<string, bool> boolValueAttribute, int column)
        {
            double gain = TinhGain(data);
            List<string> temp = new List<string>();
            for (int i = 1; i < data[column].Count(); i++)
            {
                if (!temp.Contains(data[column][i]))
                {
                    gain -= tinhEntropy(data, ref boolValueAttribute, column, data[column][i]);
                    temp.Add(data[column][i]);
                }
            }
            //Print

            //string flog = gain.ToString().Substring(0,6);
            txtID3_Algorithm.Text += "+ Gain(" + node + "," + data[column][0] + ") = " + gain + "\r\n";

            return gain;
        }


        //Trả về vị trí Best Attribute
        private int Result_Best_Attribute(string node, List<List<string>> data, ref Dictionary<string, bool> boolValueAttribute)
        {
            double bestGain = -99f;
            int indexBestAttribute = -1;
            for (int i = 0; i < data.Count() - 1; i++)
            {
                double temp = tinhGiaTriGain(node, data, ref boolValueAttribute, i);
                if (temp > bestGain)
                {
                    indexBestAttribute = i;
                    bestGain = temp;
                }
            }
            //Print
            txtID3_Algorithm.Text += "Max Gain: " + data[indexBestAttribute][0] + "\r\n";

            return indexBestAttribute;
        }


        private void DecisionTree(string node, ref NodeChuaLa tree, List<List<string>> data, ref Dictionary<string, bool> boolValueAttribute)
        {
            //Print
            txtID3_Algorithm.Text +=  "--   Xét " + node + "  \r\n";

            List<List<string>> tempData = new List<List<string>>();
            tempData = data;
            int indexBestAttribute = Result_Best_Attribute(node, tempData, ref boolValueAttribute);
            tree.name = tempData[indexBestAttribute][0];
            tree.nodeRoot = false;
            tree.value = null;
            tree.arrNodes = new List<NodeChuaLa>();
            List<string> tmpName = new List<string>();
            bool stopFunction = true;
            int index = 0, numNode = 0;
            for (int i = 1; i < tempData[indexBestAttribute].Count(); i++)
            {
                if (!tmpName.Contains(tempData[indexBestAttribute][i]))
                {
                    tmpName.Add(tempData[indexBestAttribute][i]);
                    if (checkNodeLa(boolValueAttribute, tempData[indexBestAttribute][0] + tempData[indexBestAttribute][i]))
                    {
                        //Print
                        txtID3_Algorithm.Text += "--    Xét: " +  tempData[indexBestAttribute][i] + "  \r\n";

                        txtID3_Algorithm.Text += "Tất cả: " + tempData[tempData.Count() - 1][i] + " => " + tempData[indexBestAttribute][i] + "\t KQ: "+tempData[tempData.Count() - 1][i] + "\r\n";
                        //Nếu giá trị đang xét của Attribute chứa node lá 
                        NodeChuaLa tmp = new NodeChuaLa();
                        tmp.name = tempData[indexBestAttribute][i];
                        tmp.value = tempData[tempData.Count() - 1][i];
                        tree.arrNodes.Add(tmp);
                        index++;
                    }
                    else
                    {
                        //Nếu giá trị đang xét của Attribute chứa node cành
                        NodeChuaLa tmp = new NodeChuaLa();
                        tmp.name = tempData[indexBestAttribute][i];
                        tmp.value = null;
                        tmp.arrNodes = new List<NodeChuaLa>();
                        tmp.arrNodes.Add(new NodeChuaLa());
                        tree.arrNodes.Add(tmp);
                        stopFunction = false;
                        List<List<string>> tempData_cop = new List<List<string>>();
                        tempData_cop = UpdateData(tempData, indexBestAttribute, tempData[indexBestAttribute][i]);
                        NodeChuaLa tempNode = tree.arrNodes[index];
                        DecisionTree(tmp.name, ref tempNode, tempData_cop, ref boolValueAttribute);
                        tree.arrNodes[index].arrNodes[tree.arrNodes[index].arrNodes.Count() - 1] = tempNode;
                        index++;
                    }

                }
            }
            //Nếu tất cả giá trị của thuộc tính Attribute đều là node lá ==> dừng
            if (stopFunction)
                return;
        }

        private List<List<string>> UpdateData(List<List<string>> data, int column, string name)
        {
            List<List<string>> result = new List<List<string>>();
            int ii = 0;
            for (int i = 0; i < data.Count(); i++)
            {
                if (i != column)
                {
                    result.Add(new List<string>());
                    for (int j = 0; j < data[i].Count(); j++)
                        result[ii].Add(data[i][j]);
                    ii++;
                }
            }

            for (int i = data[column].Count() - 1; i > 0; i--)
            {
                if (data[column][i] != name)
                {
                    for (int j = 0; j < result.Count(); j++)
                    {
                        result[j].RemoveAt(i);
                    }
                }
            }

            return result;
        }

        private void ShowRules(NodeChuaLa decisionTree, bool start, string rule)
        {
            string temp = "";
            if (start)
            {
                rule += "Nếu (" + decisionTree.name + "=";
            }
            else
                rule += "Và  (" + decisionTree.name + "=";
            temp = rule;
            for (int i = 0; i < decisionTree.arrNodes.Count(); i++)
            {
                rule += decisionTree.arrNodes[i].name + ") ";
                NodeChuaLa NodeChuaLa = decisionTree.arrNodes[i];
                if (NodeChuaLa.value == null) //nếu giá trị của Attribute ko chứa node lá => chứa 1 node Attribute khác(node nhánh)
                {
                    ShowRules(NodeChuaLa.arrNodes[0], false, rule);
                    rule = temp;
                }
                else
                {
                    rule += "Thì " + arrClause[arrClause.Count - 1] + "=" + NodeChuaLa.value + "\r\n";
                    txtID3_Algorithm.Text += rule;
                    rule = temp;
                }
            }
        }

        private void btnOpenExcelSource_Click(object sender, EventArgs e)
        {
            try
            {
                this.CenterToParent();
                decisionTree = new NodeChuaLa();
                arrClause = new List<string>();
                dataSource = new List<List<string>>();
                // lay du lieu tu data view

                if (dataGridViewClauses.Columns.Count < 1)
                {
                    MessageBox.Show("Không có dữ liệu mẫu để chạy thuật toán", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                dtSource = new List<List<string>>();
                for (int i = 0; i < dataGridViewClauses.Columns.Count; i++)
                {
                    dtSource.Add(new List<string>());
                    dtSource[i].Add(dataGridViewClauses.Columns[i].Name);
                    for (int j = 0; j < dataGridViewClauses.Rows.Count - 1; j++)
                    {
                        dtSource[i].Add(dataGridViewClauses.Rows[j].Cells[i].Value.ToString());
                    }
                }
                dataSource = dtSource;

               // dataSource = frmHome.dtSource;
                arrValue = new List<string>();
                int numRow = dataSource.Count();
                valueAttribute = new Dictionary<string, double>();
                boolValueAttribute = new Dictionary<string, bool>();
                foreach (var item in dataSource)
                {
                    arrClause.Add(item[0]);
                }
                for (int i = 1; i < dataSource[numRow - 1].Count(); i++)
                {
                    if (!arrValue.Contains(dataSource[numRow - 1][i]))
                    {
                        arrValue.Add(dataSource[numRow - 1][i]);
                    }
                }
                DecisionTree(" ", ref decisionTree, dataSource, ref boolValueAttribute);
                txtID3_Algorithm.Text += "\r\n\r\n-- Luật \r\n\r\n";
                ShowRules(decisionTree, true, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tính với định dạng nguồn", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           // DrawDecisionTree(decisionTree, grs, new Point(1200,0), true, 750);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //add nhóm tuổi
            comboTuoi.Items.Add("<18");
            comboTuoi.Items.Add("18_35");
            comboTuoi.Items.Add("35_50");
            comboTuoi.Items.Add("Tren_50");
            //add nhóm nghề
            comboNghe.Items.Add("N1");
            comboNghe.Items.Add("N2");
            comboNghe.Items.Add("N3");
            comboNghe.Items.Add("N4");
            //add nhóm lương 
            comboThuNhap.Items.Add("0_10");
            comboThuNhap.Items.Add("10_16");
            comboThuNhap.Items.Add("16_27");
            comboThuNhap.Items.Add("27_41");
            comboThuNhap.Items.Add("41_62");
            comboThuNhap.Items.Add("Tren_62");
            //add nhóm nợ
            comboNo.Items.Add("0");
            comboNo.Items.Add("1");
            comboNo.Items.Add("Tren_1");
        }

        private void butKetQua_Click(object sender, EventArgs e)
        {
            if (comboNo.Text == "Tren_1")
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " không đủ điều kiện được vay khi bạn thuộc nhóm nợ " + comboNo.Text);
            }
            else if (comboNo.Text == "1" && checkNha.Checked == false)
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " không đủ điều kiện được vay khi bạn thuộc nhóm nợ " + comboNo.Text + " nhưng lại " +
                    "không có nhà để thể chấp");
            }
            else if (comboTuoi.Text == "<18")
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " chưa đủ tuổi để được vay(>=18) ");
            }
            else if (comboNo.Text == "0" && comboTuoi.Text == "18_35" && checkNha.Checked == false && comboThuNhap.Text == "16_27")
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " không đủ điều kiện vay khi với mức thu nhập này thì cần phải có nhà để thế chấp ");
            }
            else if (comboNo.Text == "0" && comboTuoi.Text == "18_35" && checkNha.Checked == false && comboThuNhap.Text == "10_16")
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " không đủ điều kiện vay khi với mức thu nhập này thì cần phải có nhà để thế chấp ");
            }
            else if (comboNo.Text == "0" && comboTuoi.Text == "35_50" && checkNha.Checked == false)
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " không đủ điều kiện được vay khi bạn không có nhà để thế chấp ");
            }
            else if (comboNo.Text == "0" && comboTuoi.Text == "Tren_50" && comboThuNhap.Text == "16_27" && checkNha.Checked == false)
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " không đủ điều kiện vay khi với mức thu nhập này thì cần phải có nhà để thế chấp ");
            }
            else if (comboThuNhap.Text == "27_41" || comboThuNhap.Text == "41_62" || comboThuNhap.Text == "Tren_62" && checkXe.Checked == true && comboNo.Text == "0")
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " đủ điều kiện để được vay ");
            }
            else
            {
                MessageBox.Show("Khách hàng " + textTen.Text + " đủ điều kiện để được vay ");
            }
            textTen.Clear();
            checkHonNhan.Focus();
            checkNha.Focus();
            checkXe.Focus();
            comboTuoi.Focus();
            comboThuNhap.Focus();
            comboNo.Focus();
            comboNghe.Focus();
        }

        private void comboTuoi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
