using Terminal.Gui;

Application.Init();
var window = new Window("TimeSheet - Ctrl-Q to quit")
{
   X = 0,
   Y = 0,
   Width = Dim.Fill(),
   Height = Dim.Fill(),
};

Application.Top.Add(window);

var messageButton = new Button(1, 1, "Message");
var closeButton = new Button(12, 1, "Close", true);

messageButton.Clicked += () =>
{
   var n = MessageBox.Query(50, 7,
            "Question", "Do you want to exit?", "Yes", "No");

   if (n == 0)
   {
      Environment.Exit(0);
   }
};

closeButton.Clicked += () => Environment.Exit(0);

window.Add(messageButton);
window.Add(closeButton);

Application.Run();