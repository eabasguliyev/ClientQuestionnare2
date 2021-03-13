using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        private User _currentUser;

        private bool _isLoaded = false;
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
            if(!CheckUserInputs())
            {
                return;
            }

            if (!_isLoaded)
            {
                _currentUser = new User();
            }
            else
            {
                _isLoaded = false;
                File.Delete(FileHelper.CreateFileName(_currentUser));
            }

            SecondButtonLocations();

            _currentUser.FirstName = FirstNameTxtB.Text;
            _currentUser.LastName = LastNameTxtB.Text;
            _currentUser.FullName = $"{_currentUser.FirstName} {_currentUser.LastName}";
            _currentUser.Email = EmailTxtB.Text;
            _currentUser.Phone = PhoneTxtB.Text;
            _currentUser.Birthdate = BirthDateDT.Value;

            Users.Add(_currentUser);
            LoadUsersToListBox();
            SelectCurrentUser();
            //ChangeButtonsVisibility();
        }

        private void DefaultButtonLocations()
        {
            AddBtn.Size = new Size(195, 42);
            AddBtn.Location = new Point(119, 258);

            ModifyBtn.Size = new Size(94, 42);
            ModifyBtn.Location = new Point(220, 302);
            ModifyBtn.Visible = false;

            ClearBtn.Size = new Size(96, 42);
            ClearBtn.Location = new Point(119, 302);
            ClearBtn.Visible = false;
        }

        private void SecondButtonLocations()
        {
            AddBtn.Size = new Size(94, 42);
            AddBtn.Location = new Point(219, 302);

            ModifyBtn.Size = new Size(195,42);
            ModifyBtn.Location = new Point(118, 258);
            ModifyBtn.Visible = true;

            ClearBtn.Size = new Size(96, 42);
            ClearBtn.Location = new Point(118, 302);
            ClearBtn.Visible = true;
        }
        private void ChangeButtonsVisibility()
        {
            AddBtn.Visible = !AddBtn.Visible;
            ModifyBtn.Visible = !ModifyBtn.Visible;
        }

        private bool CheckUserInputs()
        {
            if (!ValidateUserInputs())
            {
                MessageBox.Show("User can not be added. Please, fill inputs correct format!");
                return false;
            }

            return true;
        }

        private void LoadUsersToListBox()
        {
            UsersListB.Items.Clear();
            UsersListB.ValueMember = "FullName";
            foreach (var user in Users)
            {
                UsersListB.Items.Add(user);
            }
        }

        private void SelectCurrentUser()
        {
            var userIndex = Users.IndexOf(_currentUser);
            UsersListB.SelectedIndex = userIndex;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var user = UsersListB.SelectedItem as User;

            FileHelper.WriteUserToJson(FileNameTxtB.Text, user);
        }

        private void UsersListB_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentUser = UsersListB.SelectedItem as User;

            LoadUserToForm(_currentUser);
            this.FileNameTxtB.Text = FileHelper.CreateFileName(_currentUser);
            this.FileNameTxtB.ForeColor = Color.Black;
            
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

            if (CheckUserFromList(user))
            {
                MessageBox.Show($"User already in the list");
                return;
            }

            SecondButtonLocations();
            _currentUser = user;
            _isLoaded = true;

            Users.Add(_currentUser);
            LoadUsersToListBox();
            try
            {
                LoadUserToForm(_currentUser);
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
            _currentUser.FirstName = FirstNameTxtB.Text;
            _currentUser.LastName = LastNameTxtB.Text;
            _currentUser.FullName = $"{_currentUser.FirstName} {_currentUser.LastName}";
            _currentUser.Email = EmailTxtB.Text;
            _currentUser.Phone = PhoneTxtB.Text;
            _currentUser.Birthdate = BirthDateDT.Value;

            LoadUsersToListBox();
            SelectCurrentUser();
        }

        public bool ValidateUserInputs()
        {
            var status = true;
            if (string.IsNullOrWhiteSpace(this.FirstNameTxtB.Text))
            {
                FormHelper.RedRequiredLabel(FirstNameRequiredLbl);
                FormHelper.ShowErrorLabel(FirstNameStatusLbl, "First name can not be blank!");
                status = false;
            }
            else
            {
                FormHelper.BlackRequiredLabel(FirstNameRequiredLbl);

                if (!UserHelper.CheckFirstName(FirstNameTxtB.Text))
                {
                    FormHelper.ShowErrorLabel(FirstNameStatusLbl, "First name must be min 3 characters!");
                    status = false;
                }
                else
                {
                    FormHelper.HideErrorLabel(FirstNameStatusLbl);
                }
            }

            if (string.IsNullOrWhiteSpace(this.LastNameTxtB.Text))
            {
                FormHelper.RedRequiredLabel(LastNameRequiredLbl);
                FormHelper.ShowErrorLabel(LastNameStatusLbl, "Last name can not be blank!");
                status = false;
            }
            else
            {
                FormHelper.BlackRequiredLabel(LastNameRequiredLbl);

                if (!UserHelper.CheckLastName(LastNameTxtB.Text))
                {
                    FormHelper.ShowErrorLabel(LastNameStatusLbl, "Last name must be min 5 characters!");
                    status = false;
                }
                else
                {
                    FormHelper.HideErrorLabel(LastNameStatusLbl);
                }
            }

            if (string.IsNullOrWhiteSpace(this.EmailTxtB.Text))
            {
                FormHelper.RedRequiredLabel(EmailRequiredLbl);
                FormHelper.ShowErrorLabel(EmailStatusLbl, "Email address can not be blank!");
                status = false;
            }
            else
            {
                FormHelper.BlackRequiredLabel(EmailRequiredLbl);

                if (!UserHelper.CheckMail(EmailTxtB.Text))
                {
                    FormHelper.ShowErrorLabel(EmailStatusLbl, "Invalid email format.");
                    status = false;
                }
                else
                {
                    FormHelper.HideErrorLabel(EmailStatusLbl);
                }
            }
            

            if (PhoneTxtB.Text != "(+   )   -" && !PhoneTxtB.MaskFull)
            {
                FormHelper.ShowErrorLabel(PhoneStatusLbl, "Phone number format is not valid.");
                status = false;
            }
            else
            {
                FormHelper.HideErrorLabel(PhoneStatusLbl);
            }

            return status;
        }

        private void FileNameTxtB_MouseEnter(object sender, EventArgs e)
        {
            if (FileNameTxtB.Text == "File Name")
            {
                FileNameTxtB.Text = string.Empty;
                FileNameTxtB.ForeColor = Color.Black;
            }

        }

        private void FileNameTxtB_MouseLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FileNameTxtB.Text))
            {
                FileNameTxtB.Text = "File Name";
                FileNameTxtB.ForeColor = Color.Gray;
            }
        }

        private bool CheckUserFromList(User user)
        {
            return Users.Any(u => u.Guid == user.Guid);
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            _currentUser = null;
            ClearUserInputs();
            DefaultButtonLocations();
        }

        private void ClearUserInputs()
        {
            FirstNameTxtB.Text = String.Empty;
            LastNameTxtB.Text = String.Empty;
            EmailTxtB.Text = String.Empty;
            PhoneTxtB.Text = String.Empty;
            BirthDateDT.Text = String.Empty;
        }
    }
}