using System;
using System.Data.SqlClient;

namespace Twitter
{
    class Program
    {
        public int currentUser = 0;
        public string username = "";
        public string menuChoice = "";
        public string userDisplay = "";
        public string sqlString = "";
        public bool signedIn = false;
        public bool exit = false;
            //need to prevent it creating multiple users
        public void CreateUser(SqlConnection connection)
        {
            Console.WriteLine("Enter new username: \n");
            username = Console.ReadLine();
            sqlString = $"SELECT * FROM Users WHERE username = '{username}'";
            SqlCommand command = new SqlCommand(sqlString, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine("That username already exists. Is that you (y/n)?");
                if (Console.ReadLine() != "y")
                {
                    Console.WriteLine("Please enter a different name: ");
                    username = Console.ReadLine();
                }
                else
                {
                    currentUser = Convert.ToInt16(reader["Id"]);
                }
            }
            reader.Close();
            Console.WriteLine("Create custom display name? (y/n)");
            menuChoice = Console.ReadLine();

            if (menuChoice == "y")
            {
                Console.WriteLine("Enter display name: ");
                userDisplay = Console.ReadLine();
            }
            else
            {
                userDisplay = username;
            }

            sqlString = "INSERT INTO Users (username, displayname) VALUES (@username, @display)";command = new SqlCommand(sqlString, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@display", userDisplay); 
            command.ExecuteNonQuery();
            
            sqlString = $"SELECT Id FROM Users WHERE username = '{username}'";
            command = new SqlCommand(sqlString, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                currentUser = Convert.ToInt16(reader["Id"]);
            }
            reader.Close();
            Console.WriteLine("User created!");
            signedIn = true;
            //return username;
        }

        public void LogIn(SqlConnection connection)
        {
            Console.WriteLine("Enter username: ");
            username = Console.ReadLine();
            SqlCommand logIn = new SqlCommand($"SELECT Id, displayname FROM Users WHERE username = '{username}'", connection);
            SqlDataReader reader = logIn.ExecuteReader();

            if (reader.Read())
            {
                currentUser = Convert.ToInt16(reader["Id"]);
                userDisplay = Convert.ToString(reader["displayname"]);
                signedIn = true;
            }
            else
            {
                Console.WriteLine("Username not recognized.\n1. Continue as guest  |  2. Create a new account.");
                menuChoice = Console.ReadLine().ToLower();
                if (menuChoice == "1")
                {
                    GuestMenu(connection);
                }
                else if (menuChoice == "2")
                {
                    CreateUser(connection);
                }
                else
                {
                    Console.WriteLine("Please choose: 1. Continue as guest  |  2. Create a new account.");
                }
            }
            reader.Close();
            //return username;
        }

        public bool SignInMenu(SqlConnection connection)
        {
            Console.WriteLine("w e l c o m e  t o\nT  W  I  T  T  E  R\n");
            Console.WriteLine("1. Sign in  |  2. Create Account  |  3. Use as guest  |  4. Exit");
            menuChoice = Console.ReadLine();
            if (menuChoice == "1")
            {
                LogIn(connection);
            }
            else if (menuChoice == "2")
            {
                CreateUser(connection);
            }
            else if (menuChoice == "3")
            {
                GuestMenu(connection);
            }
            else
            {
                Console.WriteLine("Answer not recognized. Try again.");
                Console.Clear();
                SignInMenu(connection);
            }
            return signedIn;
        }

        public void DisplayFeed(SqlConnection connection)
        {
            Console.WriteLine("---------------------------------------------\nPress ENTER to return to menu");
            Console.WriteLine("---------------------------------------------\n");
            sqlString = "SELECT * FROM Tweets JOIN Users ON Users.Id = Tweets.userID";
            SqlCommand command = new SqlCommand(sqlString, connection);
            SqlDataReader reader2 = command.ExecuteReader();
            while (reader2.Read())
            {
                Console.WriteLine($"{reader2["displayname"]} (@{reader2["username"]}):\n");
                Console.WriteLine(reader2["text"] + "\n");
                Console.WriteLine($"{reader2["date_time"]}");
                Console.WriteLine("-----------------------------------------\n");
            }
            reader2.Close();
            if (Console.ReadLine() != null)
            {
                Console.Clear();
                if (signedIn == true)
                {
                    UserMenu(connection);
                }
                else
                {
                    GuestMenu(connection);
                }
            }
        }

        public void GuestMenu(SqlConnection connection)
        {
            Console.WriteLine($"Hi!\nYou are signed in as a guest.");
            Console.WriteLine("T  W  I  T  T  E  R\n1. See Feed |  2. Search |  3. Create Account  |  4. Exit ");
            menuChoice = Console.ReadLine();

            if (menuChoice == "1")
            {
                DisplayFeed(connection);
            }
            else if (menuChoice == "2")
            {
                Search(connection);
            }

            else if (menuChoice == "3")
            {
                CreateUser(connection);
            }
            else if (menuChoice == "4")
            {
                Exit(connection);
            }
        }

        public void UserMenu(SqlConnection connection)
        {
            Console.WriteLine($"Hi, {username}.\n");
            Console.WriteLine("T  W  I  T  T  E  R\n1. See Feed | 2. Tweet | 3. Search | 4. Settings | 5. Exit ");
            menuChoice = Console.ReadLine();

            if (menuChoice == "1")
            {
                DisplayFeed(connection);
            }
            else if (menuChoice == "2")
            {
                Tweet(connection, currentUser);
            }

            else if (menuChoice == "3")
            {
                Search(connection);
            }
            else if (menuChoice == "5")
            {
                Exit(connection);
            }
        }

        public void Tweet(SqlConnection connection, int currentUser)
        {
            Console.WriteLine("Enter tweet (280 characters): ");
            string newTweet = Console.ReadLine();
            sqlString = $"INSERT INTO Tweets (userID,text,date_time) VALUES (@id, @tweet, @time)";
            SqlCommand command = new SqlCommand(sqlString, connection);
            command.Parameters.AddWithValue("@tweet", newTweet);
            command.Parameters.AddWithValue("@id", currentUser);
            command.Parameters.AddWithValue("@time", DateTime.Now);

            command.ExecuteNonQuery();
            Console.Clear();
            Console.WriteLine("tweet tweeted!");
            if (signedIn == true)
            {
                UserMenu(connection);
            }
            else
            {
                GuestMenu(connection);
            }
        }

        public void Search(SqlConnection connection)
        {
            string searchTerm = "";
            Console.WriteLine("Search by 1. User or 2. Tweet Content?");
            int search = Convert.ToInt16(Console.ReadLine());
            if (search == 1)
            {
                Console.WriteLine("Enter username: ");
                searchTerm = Console.ReadLine();
                //new sqlString to search for tweets where the username equals that, have to join tables

            }
        }

        public void Exit(SqlConnection connection)
        {
            Console.Clear();
            Console.WriteLine("Goodbye!");
            exit = true;
        }
        static void Main(string[] args)
        {
            string filename = "C:\\Users\\AcademyPgh\\Documents\\Academy\\Session10c3\\Twitter\\Twitter\\Database1.mdf";
            SqlConnection connection = new SqlConnection(@$"Data Source=(LocalDb)\MSSQLLocalDB; AttachDbFilename = {filename}; Integrated Security = True;MultipleActiveResultSets=True");
            //bool exit = false;
            Program twitter = new Program();

            connection.Open();
            Console.WriteLine("-_-_-_-_-_-_- T W I T T E R -_-_-_-_-_-_-");
            Console.WriteLine("hello");

            while (twitter.exit == false)
            {
                twitter.SignInMenu(connection);
                if (twitter.signedIn == true)
                {
                    twitter.UserMenu(connection);
                }
                else
                {
                    twitter.GuestMenu(connection);
                }
            }
            connection.Close();
        }
    }
}

