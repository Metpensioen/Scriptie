using System.Windows.Controls;

using static EditImage;
using static EditText;

class GridEdit : Grid
{
    public Grid EditInit()
    {
        Children.Add(editImage.ImageInit());
        Children.Add(editText.TextInit());

        return this;
    }

    public static GridEdit gridEdit = new GridEdit();
}