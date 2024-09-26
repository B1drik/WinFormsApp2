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
        private string connectionString = @"Data Source=dbsrv\dub2024;Initial Catalog=Zolotarevavv2207b2;Integrated Security=True;TrustServerCertificate=True;"; // Замените на вашу строку подключения

        private string sortDirection = "ASC"; // Начальное направление сортировки
        private string searchTerm = ""; // Поисковый термин
        private List<string> searchTerms = new List<string>(); // Список поисковых терминов

        public Form1()
        {
            InitializeComponent();

            // Инициализация подключения к базе данных
            connection = new SqlConnection(connectionString);

            // Загрузка данных из базы данных
            LoadProducts();
        }

        private void LoadProducts()
        {
            listView1.Items.Clear(); // Очистка ListView

            try
            {
                connection.Open();

                // Создание SQL-запроса с использованием WHERE и AND
                StringBuilder queryBuilder = new StringBuilder("SELECT * FROM Products WHERE ");

                // Добавление условий поиска
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
                    queryBuilder.Append("1=1"); // Условие "истина" для всех записей
                }

                queryBuilder.Append($" ORDER BY ProductName {sortDirection}");

                // Выполнение SQL-запроса
                SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection);
                SqlDataReader reader = command.ExecuteReader();

                // Чтение данных из SqlDataReader и добавление их в ListView
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

                // Обновление текста лейбла
                UpdateRecordCountLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void UpdateRecordCountLabel()
        {
            // Обновление текста лейбла количеством записей в ListView
            label1.Text = $"Записей в ListView: {listView1.Items.Count}";
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
                string[] terms = searchTerm.Split(' '); // Разбиваем поисковую строку по пробелам
                searchTerms.AddRange(terms);
            }
            LoadProducts();
        }

    }
}