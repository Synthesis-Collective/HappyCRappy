using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace HappyCRappy;

public class MultiSelectionListBox : ListBox
{
    public IList SelectedItemsList
    {
        get { return (IList)GetValue(SelectedItemsListProperty); }
        set { SetValue(SelectedItemsListProperty, value); }
    }

    public MultiSelectionListBox() { }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        SelectedItemsList = SelectedItems;
    }

    public static readonly DependencyProperty SelectedItemsListProperty =
       DependencyProperty.Register(nameof(SelectedItemsList), typeof(IList), typeof(MultiSelectionListBox), new PropertyMetadata(null));

}