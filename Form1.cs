using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private SqlConnection connection;
        private string connectionString = @"Data Source=dbsrv\dub2024;Initial Catalog=Zolotarevavv2207b2;Integrated Security=True;TrustServerCertificate=True;"; // �������� �� ���� ������ �����������

        private string sortDirection = "ASC"; // ��������� ����������� ����������
        private string searchTerm = ""; // ��������� ������
        private List<string> searchTerms = new List<string>(); // ������ ��������� ��������

        public Form1()
        {
            InitializeComponent();

            // ������������� ����������� � ���� ������
            connection = new SqlConnection(connectionString);

            // �������� ������ �� ���� ������
            LoadProducts();
        }

        private void LoadProducts()
        {
            listView1.Items.Clear(); // ������� ListView

            try
            {
                connection.Open();

                // �������� SQL-������� � �������������� WHERE � AND
                StringBuilder queryBuilder = new StringBuilder("SELECT * FROM Products WHERE ");

                // ���������� ������� ������
                if (searchTerms.Count > 0)
                {
                    for (int i = 0; i < searchTerms.Count; i++)
                    {
                        queryBuilder.Append($"Manufacturer LIKE '%{searchTerms[i]}%'");
                        if (i < searchTerms.Count - 1)
                        {
                            queryBuilder.Append(" OR ");
                        }
                    }
                }
                else
                {
                    queryBuilder.Append("1=1"); // ������� "������" ��� ���� �������
                }

                queryBuilder.Append($" ORDER BY ProductName {sortDirection}");

                // ���������� SQL-�������
                SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection);
                SqlDataReader reader = command.ExecuteReader();

                // ������ ������ �� SqlDataReader � ���������� �� � ListView
                while (reader.Read())
                {
                    ListViewItem item = new ListViewItem(reader["ProductName"].ToString());
                    ListViewItem item1 = new ListViewItem(reader["Price"].ToString());
                    ListViewItem item2 = new ListViewItem(reader["Manufacturer"].ToString());
                    ListViewItem item3 = new ListViewItem(reader["QuantityInStock"].ToString());
                    ListViewItem item4 = new ListViewItem(reader["Description"].ToString());

                    listView1.Items.Add(item);
                    listView1.Items.Add(item1);
                    listView1.Items.Add(item2);
                    listView1.Items.Add(item3);
                    listView1.Items.Add(item4);
                }

                reader.Close();

                // ���������� ������ ������
                UpdateRecordCountLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("������: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void UpdateRecordCountLabel()
        {
            // ���������� ������ ������ ����������� ������� � ListView
            label1.Text = $"������� � ListView: {listView1.Items.Count}";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                sortDirection = "DESC";
                LoadProducts();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                sortDirection = "ASC";
                LoadProducts();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            searchTerm = textBox1.Text;
            searchTerms.Clear();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string[] terms = searchTerm.Split(' '); // ��������� ��������� ������ �� ��������
                searchTerms.AddRange(terms);
            }
            LoadProducts();
        }

    }
}