using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Library_worrk
{
    public partial class Form1 : Form
    {
        private DbConnection conn;
        int number = 0;
        int tick = 0;
        long id_lib;
        long studentId = 0;
        long teacherId = 0;
        long bookid = 0;
        long id_scards = 0;
        long id_tcards = 0;

        public Form1()
        {
            InitializeComponent();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "KOLYAN\\NIKOLAY";
            builder.InitialCatalog = "Library_work";
            //builder.UserID = "sa";
            //builder.Password = "ghbvf";
            builder.IntegratedSecurity = true;
            conn = new SqlConnection(builder.ToString());
            tabPage2.Enabled = false;
            tabPage3.Enabled = false;
            tabPage4.Enabled = false;
            tabPage5.Enabled = false;
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;
            comboBox5.Enabled = false;
            comboBox6.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            //tabPage4.Enabled = false;
        }
        private class Faculty
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        private class Group
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
        private class Departmens
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
        private class Teachers
        {
            public long Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public override string ToString()
            {
                return FirstName + " " + LastName;
            }
        }

        private class AuthorBook
        {
            public long Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public override string ToString()
            {
                return FirstName + " " + LastName;
            }
        }

        private class Category
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        private class Theme
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
        private class Press
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DbCommand command = conn.CreateCommand();

                DbParameter login_param = command.CreateParameter();
                DbParameter Password_param = command.CreateParameter();
                DbParameter name_param = command.CreateParameter();
                DbParameter id_param = command.CreateParameter();

                login_param.ParameterName = "login";
                login_param.DbType = DbType.String;
                login_param.Size = 255;

                Password_param.ParameterName = "passwd";
                Password_param.DbType = DbType.String;
                Password_param.Size = 255;

                

                name_param.ParameterName = "name";
                name_param.DbType = DbType.String;
                name_param.Size = 60;

                id_param.ParameterName = "id";
                id_param.DbType = DbType.Int64;


                login_param.Value = textBox1.Text;
                Password_param.Value = textBox2.Text;
                name_param.Direction = ParameterDirection.Output;
                id_param.Direction = ParameterDirection.Output;

                command.Parameters.Add(login_param);
                command.Parameters.Add(Password_param);
                command.Parameters.Add(name_param);
                command.Parameters.Add(id_param);

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Log_in";

                
                command.ExecuteNonQuery();
                var name = name_param.Value as String;
                id_lib = (long)id_param.Value;

                if (name == null)
                {
                    MessageBox.Show("Вы не прошли авторизацию!");
                    Close();
                }
                else
                {
                    label3.Text = "Добро пожалувать в библеотеку: " + name_param.Value.ToString();
                    button2.Enabled = true;
                    button3.Enabled = true;
                }
            }

            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }    
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'library_workDataSet.Groups' table. You can move, or remove it, as needed.
            this.groupsTableAdapter.Fill(this.library_workDataSet.Groups);
            // TODO: This line of code loads data into the 'library_workDataSet.Students' table. You can move, or remove it, as needed.
            this.studentsTableAdapter.Fill(this.library_workDataSet.Students);
            try
            {
                conn.Open();
                FillFaculties();
                FillDepartment();
                FillAuthors();
                FillCategory();
                FillTheme();
                FillPress();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFaculties()
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "SELECT Faculties.Id, Faculties.Name FROM Faculties";
                DbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    comboBox1.Items.Add(
                        new Faculty
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });

                }
                reader.Close();
                if (comboBox1.SelectedIndex > 0)
                {
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void FillGroups(long Id)
        {
            try
            {
                listBox1.Items.Clear();
                DbCommand command = conn.CreateCommand();
                DbParameter faculty_param = command.CreateParameter();
                faculty_param.ParameterName = "facultyId";
                faculty_param.DbType = DbType.Int64;
                faculty_param.Value = Id;
                command.Parameters.Add(faculty_param);

                command.CommandText = "SELECT Groups.Id, Groups.Name FROM Groups WHERE FacultyId = @facultyId";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listBox1.Items.Add(
                        new Group
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                }
                reader.Close();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty faculty = comboBox1.SelectedItem as Faculty;

            if (faculty != null)
            {
                FillGroups(faculty.Id);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Group group = listBox1.SelectedItem as Group;

            if (group != null)
            {
                FillStudents(group.Id);
            }
        }

        private void FillStudents(long Id)
        {
            try
            {
                listView1.Items.Clear();
                DbCommand command = conn.CreateCommand();
                DbParameter group = command.CreateParameter();

                group.ParameterName = "groupId";
                group.DbType = DbType.Int32;
                group.Value = Id;
                command.Parameters.Add(group);

                command.CommandText = "SELECT Students.id, Students.FirstName, Students.LastName, Students.LogbookNumber FROM Students WHERE GroupId = @groupId";
                DbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ListViewItem item = listView1.Items.Add(reader.GetString(reader.GetOrdinal("FirstName")));
                    item.SubItems.Add(reader.GetString(reader.GetOrdinal("LastName")));
                    item.SubItems.Add(reader.GetString(reader.GetOrdinal("LogbookNumber")));
                    item.Tag = reader.GetInt32(reader.GetOrdinal("id"));

                }
                reader.Close();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillS_Card(long Id)
        {
            try
            {
                listView2.Items.Clear();
                DbCommand command = conn.CreateCommand();
                DbParameter students = command.CreateParameter();
                students.ParameterName = "studentId";
                students.DbType = DbType.Int64;
                students.Value = Id;
                command.Parameters.Add(students);

                command.CommandText = "SELECT S_Cards.Id, S_Cards.DateIn, S_Cards.DateOut, S_Cards.BookId, S_Cards.LibId FROM S_Cards WHERE student_Id = @studentId";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ListViewItem item = listView2.Items.Add(reader.GetDateTime(reader.GetOrdinal("DateIn")).ToString());
                    item.SubItems.Add(reader.GetDateTime(reader.GetOrdinal("DateOut")).ToString());
                    item.SubItems.Add(reader.GetInt64(reader.GetOrdinal("BookId")).ToString());
                    item.SubItems.Add(reader.GetInt64(reader.GetOrdinal("LibId")).ToString());
                    item.Tag = reader.GetInt64(reader.GetOrdinal("Id"));
                }
                reader.Close();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                int id = (int)item.Tag;
                FillS_Card(id);
                studentId = id;
                button4.Enabled = true;
            }
        }

        private void FillDepartment()
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "SELECT Departmens.Id, Departmens.Name FROM Departmens";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox2.Items.Add(
                        new Departmens
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                }
                reader.Close();
                if (comboBox2.SelectedIndex > 0)
                {
                    comboBox2.SelectedIndex = 0;
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Departmens department = comboBox2.SelectedItem as Departmens;
            if (department != null)
            {
                FillTeachers(department.Id);
            }
        }

        private void FillTeachers(long Id)
        {
            try
            {
                listBox2.Items.Clear();
                DbCommand command = conn.CreateCommand();
                DbParameter dep_id = command.CreateParameter();
                dep_id.ParameterName = "dep_id";
                dep_id.DbType = DbType.Int64;
                dep_id.Value = Id;
                command.Parameters.Add(dep_id);
                command.CommandText = "SELECT Teachers.Id, Teachers.FirstName, Teachers.LastName FROM Teachers WHERE department_Id = @dep_id";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listBox2.Items.Add(
                        new Teachers
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });
                }
                reader.Close();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Teachers teach = listBox2.SelectedItem as Teachers;
            if (teach != null)
            {
                FillT_Cards(teach.Id);
                teacherId = teach.Id;
                button5.Enabled = true;
            }
        }

        private void FillT_Cards(long Id)
        {
            try
            {
                listView3.Items.Clear();
                DbCommand command = conn.CreateCommand();
                DbParameter teach_id = command.CreateParameter();
                teach_id.ParameterName = "teach_id";
                teach_id.DbType = DbType.Int64;
                teach_id.Value = Id;
                command.Parameters.Add(teach_id);
                command.CommandText = "SELECT T_Cards.Id, T_Cards.DateIn, T_Cards.DateOut, T_Cards.BookId, T_Cards.LibId FROM T_Cards WHERE teacher_Id = @teach_id";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ListViewItem item = listView3.Items.Add(reader.GetDateTime(reader.GetOrdinal("DateIn")).ToString());
                    item.SubItems.Add(reader.GetDateTime(reader.GetOrdinal("DateOut")).ToString());
                    item.SubItems.Add(reader.GetInt64(reader.GetOrdinal("BookId")).ToString());
                    item.SubItems.Add(reader.GetInt64(reader.GetOrdinal("LibId")).ToString());
                    item.Tag = reader.GetInt64(reader.GetOrdinal("Id"));
                }
                reader.Close();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillAuthors()
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "SELECT Authors.Id, Authors.FirstName, Authors.LastName FROM Authors";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox3.Items.Add(
                        new AuthorBook
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });
                }
                reader.Close();
                if (comboBox3.SelectedIndex > 0)
                {
                    comboBox3.SelectedIndex = 0;
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillCategory()
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "SELECT Categories.Id, Categories.Name FROM Categories";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox4.Items.Add(
                        new Category
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                }
                reader.Close();
                if (comboBox4.SelectedIndex > 0)
                {
                    comboBox4.SelectedIndex = 0;
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillTheme()
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "SELECT Themes.Id, Themes.Name FROM Themes";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox5.Items.Add(
                        new Theme
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                }
                reader.Close();
                if (comboBox5.SelectedIndex > 0)
                {
                    comboBox5.SelectedIndex = 0;
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillPress()
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "SELECT Press.Id, Press.Name FROM Press";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox6.Items.Add(
                        new Press
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                }
                reader.Close();
                if (comboBox6.SelectedIndex > 0)
                {
                    comboBox6.SelectedIndex = 0;
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            number = 1;
            AuthorBook author = comboBox3.SelectedItem as AuthorBook;
            
            if (author != null)
            {
                FillBooks(author.Id);
            }

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            number = 2;
            Category categ = comboBox4.SelectedItem as Category;
            if (categ != null)
            {
                FillBooks(categ.Id);
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            number = 3;
            Theme th = comboBox5.SelectedItem as Theme;
            if (th != null)
            {
                FillBooks(th.Id);
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            number = 4;
            Press pr = comboBox6.SelectedItem as Press;
            if (pr != null)
            {
                FillBooks(pr.Id);
            }
        }

        private void FillBooks(long Id)
        {
            try
            {
                listView4.Items.Clear();
                DbCommand command = conn.CreateCommand();
                DbParameter bookId = command.CreateParameter();
                bookId.ParameterName = "bookId";
                bookId.DbType = DbType.Int64;
                bookId.Value = Id;
                command.Parameters.Add(bookId);

                switch (number)
                {
                    case 1:
                        command.CommandText = "SELECT Books.Id, Books.ISBN, Books.Name, Books.Years, Books.CategoryId, Books.PressId, Books.ThemeId, Books.Quantity FROM Books INNER JOIN AuthorBook on AuthorBook.CategoryId = Books.CategoryId WHERE AuthorId = @bookId";
                        break;
                    case 2:
                        command.CommandText = "SELECT Books.Id, Books.ISBN, Books.Name, Books.Years, Books.CategoryId, Books.PressId, Books.ThemeId, Books.Quantity FROM Books WHERE CategoryId = @bookId";
                        break;
                    case 3:
                        command.CommandText = "SELECT Books.Id, Books.ISBN, Books.Name, Books.Years, Books.CategoryId, Books.PressId, Books.ThemeId, Books.Quantity FROM Books WHERE ThemeId = @bookId";
                        break;
                    case 4:
                        command.CommandText = "SELECT Books.Id, Books.ISBN, Books.Name, Books.Years, Books.CategoryId, Books.PressId, Books.ThemeId, Books.Quantity FROM Books WHERE PressId = @bookId";
                        break;
                    default:
                        MessageBox.Show("Ошибка!");
                        break;
                }
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ListViewItem item = listView4.Items.Add(reader.GetString(reader.GetOrdinal("ISBN")));
                    item.SubItems.Add(reader.GetString(reader.GetOrdinal("Name")));
                    item.SubItems.Add(reader.GetValue(reader.GetOrdinal("Years")).ToString());
                    item.SubItems.Add(reader.GetInt64(reader.GetOrdinal("CategoryId")).ToString());
                    item.SubItems.Add(reader.GetInt64(reader.GetOrdinal("PressId")).ToString());
                    item.SubItems.Add(reader.GetInt64(reader.GetOrdinal("ThemeId")).ToString());
                    item.SubItems.Add(reader.GetInt32(reader.GetOrdinal("Quantity")).ToString());
                    item.Tag = reader.GetInt64(reader.GetOrdinal("Id"));
                }
                reader.Close();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                comboBox3.Enabled = true;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
            }
            if (radioButton2.Checked)
            {
                comboBox3.Enabled = false;
                comboBox4.Enabled = true;
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
            }
            if (radioButton3.Checked)
            {
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = true;
                comboBox6.Enabled = false;
            }
            if (radioButton4.Checked)
            {
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                comboBox6.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            tick = 1;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            switch (tick)
            {
                case 1:
                    tabControl1.SelectedIndex = 1;
                    tabPage2.Enabled = true;
                    break;
                case 2:
                    tabControl1.SelectedIndex = 2;
                    tabPage3.Enabled = true;
                    break;
                case 3:
                    tabControl1.SelectedIndex = 3;
                    tabPage4.Enabled = true;
                    break;
                case 4:
                    tabControl1.SelectedIndex = 4;
                    tabPage5.Enabled = true;
                    break;
                default:
                    MessageBox.Show("Ошибка. Неправильный индекс");
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            tick = 2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            tick = 3;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            tick = 3;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            tick = 4;
        }

        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                ListViewItem item = listView4.SelectedItems[0];
                bookid = (long) item.Tag;
                
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                if (teacherId == 0)
                {
                    get_stud_id();
                    DbParameter id_param = command.CreateParameter();
                    DbParameter datain = command.CreateParameter();
                    DbParameter dataout = command.CreateParameter();
                    DbParameter book_id = command.CreateParameter();
                    DbParameter stud_id = command.CreateParameter();
                    DbParameter lib_id = command.CreateParameter();

                    id_param.ParameterName = "id";
                    id_param.DbType = DbType.Int64;
                    id_param.Value = id_scards;

                    datain.ParameterName = "Data_in";
                    datain.DbType = DbType.DateTime;
                    datain.Value = dateTimePicker1.Value;

                    dataout.ParameterName = "Data_out";
                    dataout.DbType = DbType.DateTime;
                    dataout.Value = dateTimePicker2.Value;

                    book_id.ParameterName = "book_id";
                    book_id.DbType = DbType.Int64;
                    book_id.Value = bookid;

                    stud_id.ParameterName = "stud_id";
                    stud_id.DbType = DbType.Int64;
                    stud_id.Value = studentId;

                    lib_id.ParameterName = "lib_id";
                    lib_id.DbType = DbType.Int64;
                    lib_id.Value = id_lib;

                    command.Parameters.Add(id_param);
                    command.Parameters.Add(datain);
                    command.Parameters.Add(dataout);
                    command.Parameters.Add(book_id);
                    command.Parameters.Add(stud_id);
                    command.Parameters.Add(lib_id);

                    command.CommandText = @"INSERT INTO S_Cards(Id, DateIn, DateOut, BookId, LibId, student_Id) VALUES (@id, @Data_in, @Data_out, @book_id, @lib_id, @stud_id)";
                    if (command.ExecuteNonQuery() > 0)
                    {
                        label21.Text = "Книга успешно выдана студенту";
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при выдачи книги студенту!");
                    }
                }
                else if (studentId == 0)
                {
                    DbParameter Id_param = command.CreateParameter();
                    DbParameter datain = command.CreateParameter();
                    DbParameter dataout = command.CreateParameter();
                    DbParameter book_id = command.CreateParameter();
                    DbParameter teach_id = command.CreateParameter();
                    DbParameter lib_id = command.CreateParameter();

                    Id_param.ParameterName = "id";
                    Id_param.DbType = DbType.Int64;
                    Id_param.Value = id_tcards;


                    datain.ParameterName = "Data_in";
                    datain.DbType = DbType.DateTime;
                    datain.Value = dateTimePicker1.Value;

                    dataout.ParameterName = "Data_out";
                    dataout.DbType = DbType.DateTime;
                    dataout.Value = dateTimePicker2.Value;

                    book_id.ParameterName = "book_id";
                    book_id.DbType = DbType.Int64;
                    book_id.Value = bookid;

                    teach_id.ParameterName = "teach_id";
                    teach_id.DbType = DbType.Int64;
                    teach_id.Value = teacherId;

                    lib_id.ParameterName = "lib_id";
                    lib_id.DbType = DbType.Int64;
                    lib_id.Value = id_lib;

                    command.Parameters.Add(Id_param);
                    command.Parameters.Add(datain);
                    command.Parameters.Add(dataout);
                    command.Parameters.Add(book_id);
                    command.Parameters.Add(teach_id);
                    command.Parameters.Add(lib_id);

                    command.CommandText = @"INSERT INTO S_Cards(Id, DateIn, DateOut, BookId, LibId, student_Id) VALUES (@id, @Data_in, @Data_out, @book_id, @lib_id, @teach_id)";
                    if (command.ExecuteNonQuery() > 0)
                    {
                        label21.Text = "Книга успешно выдана преподавателю";
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при выдачи книги перподователю!");
                    }
                }
                else
                {
                    MessageBox.Show("Данной книги на данный момент нету!");
                }
                studentId = 0;
                teacherId = 0;
                id_scards = 0;
                id_tcards = 0;
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void get_stud_id()
        {
            try
            {
                
                DbCommand command = conn.CreateCommand();
                DbParameter count_param = command.CreateParameter();

                count_param.ParameterName = "id";
                count_param.DbType = DbType.Int64;
                count_param.Direction = ParameterDirection.Output;
                command.Parameters.Add(count_param);

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "get_id_scard";
                command.ExecuteNonQuery();
                id_scards = (long)count_param.Value;
                if (id_scards != 0)
                {
                    id_scards += 1;
                }
                else
                {
                    MessageBox.Show("Не удалось вычислить ID");
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void get_teach_id()
        {
            try
            {

                DbCommand command = conn.CreateCommand();
                DbParameter count_param = command.CreateParameter();

                count_param.ParameterName = "id";
                count_param.DbType = DbType.Int64;
                count_param.Direction = ParameterDirection.Output;
                command.Parameters.Add(count_param);

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "get_id_tcard";
                command.ExecuteNonQuery();
                id_tcards = (long)count_param.Value;
                if (id_tcards != 0)
                {
                    id_tcards += 1;
                }
                else
                {
                    MessageBox.Show("Не удалось вычислить ID");
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void studentsBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.studentsBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.library_workDataSet);

        }

        private void studentsBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            this.studentsBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.library_workDataSet);
        }

       
      

    }
}
