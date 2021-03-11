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

namespace _110321Task
{
    public partial class Form1 : Form
    {
        // draggable private members
        private bool _dragging = false;
        private Point _dragCursorPoint;
        private Point _dragFormPoint;

        private List<User> Users { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Users = new List<User>();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragCursorPoint = Cursor.Position;
            _dragFormPoint = this.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
                this.Location = Point.Add(_dragFormPoint, new Size(dif));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if(CheckUserInputs())
            {
                var newUser = new User();

                newUser.FirstName = FirstNameTxtB.Text;
                newUser.LastName = LastNameTxtB.Text;
                newUser.FullName = $"{newUser.FirstName} {newUser.LastName}";
                newUser.Email = EmailTxtB.Text;
                newUser.Phone = PhoneTxtB.Text;
                newUser.Birthdate = BirthDateDT.Value;

                Users.Add(newUser);
                LoadUsersToListBox();
            }
        }

        private bool CheckUserInputs()
        {
            return true;
        }

        private void LoadUsersToListBox()
        {
            UsersListB.DataSource = null;
            UsersListB.ValueMember = "FullName";
            UsersListB.DataSource = Users;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var user = UsersListB.SelectedItem as User;

            FileHelper.WriteUserToJson(FileNameTxtB.Text, user);
        }

        private void UsersListB_SelectedIndexChanged(object sender, EventArgs e)
        {
            var user = UsersListB.SelectedItem as User;

            this.FileNameTxtB.Text = FileHelper.GetFileName(user?.FirstName, user?.LastName);
        }

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            var fileName = FileNameTxtB.Text;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("File name can not be blank!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FocusToTextBox(FileNameTxtB);
                return;
            }

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"This file does not exist: {fileName}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FocusToTextBox(FileNameTxtB);
                return;
            }

            var user = FileHelper.ReadUserFromJson(fileName);


            try
            {
                LoadUserToForm(user);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void FocusToTextBox(TextBox textBox)
        {
            textBox.Focus();
        }

        private void LoadUserToForm(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            FirstNameTxtB.Text = user.FirstName;
            LastNameTxtB.Text = user.LastName;
            EmailTxtB.Text = user.Email;
            PhoneTxtB.Text = user.Phone;
            BirthDateDT.Value = user.Birthdate;
        }

        private void ModifyBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
