   M S B A M L   `   `   `  ����  STEP-corrector    STEP_corrector.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   U QWpfAnimatedGif, Version=2.0.0.0, Culture=neutral, PublicKeyToken=9e7cd3b544a090dc,gif"http://wpfanimatedgif.codeplex.com  5   	   -   5          �Name$  STEP_corrector��5   	    � Title$ STEP-corrector��5   	   $	��450��6    $	��800��6-    � Icon$ /faviconV2.png��69    � 
ResizeMode$ NoResize=�5	   	    � WindowStyle$
 None=�6    � WindowStartupLocation$ CenterScreen=�62   ��5      �� 5   
    �� ImageSource$ /background.png��6   .��5      � -   5         �Name$ MainGrid��5       � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      $��* �6   .+�5   
   � $��0��6   ���	#FF121212.+�5      �� $
��Left=�6   $��Center=�6/   $��14��6J   $��14��6U   $��/faviconV2.png��6a   $��1��6y   $��1��6�   $��10,0,0,0q�6�   $
}�True��6�   	 J� SnapsToDevicePixels$
	 True��6�   �� 5      -   5        
 ��Name$
 
TitelLabel��5      $��STEP-corrector��6(   $
��Left=�6A   $	��Top=�6\   
�������$��25,0,0,0q�6�   �� 5      -   5         ��Name$ 
ButtonQuit��5   A      CloseButtonStyle1#���   6   $��X��6U   $
��Left=�6a   $��30��6|   $��0,0,0,0q�6�   $
}�True��6�   $
	 True��6�   $��1��6�   
����������	#00000000$��20(�6  $��Corbel)�6  ���	#00DDDDDD$��25��6E  $��	770,0,0,0q�6Q  $	��Top=�6d  6|  5      � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $	��39* �6#   �� 5      .+�5"      �� -   5          ImportButtonStyle#���  5"      $��Загрузить STEP��6A   $
��Left=�6Z   $��
80,260,0,0q�5#      $	��Top=�6)   
���  ��
�������
�������$	��200��5%   (   $��60��64   $��20(�5&      $��
Constantia)�6#   6;   �� 5(      $
��Left=�6   $��
105,87,0,0q�6/   $	��Top=�6C   $	��150��6[   $��/folderIcon.png��6h   �� 5)      
���  ��
���  ��$��3��68   $	��300��6B   �� 5+      $)��#Исправить STEP-файлы:��6   $
��Left=�64   $��
424,80,0,0q�6O   $	��Top=�6c   $��18(�6{   $��
Constantia)�6�    � ToolTip$Y SОтметьте файлы, которые необходимо исправить��5.   !   5/      �� 50      -   5         ��Name$ FilesListBox��50      $	��130��6,   
�������$��1q�6L   $
��Left=�6`   $	��260��6{   $��430,117,0,178q�6�    �� SelectionMode$ Single=�6�   ��51      �� 52      .R�53      ��  �� Orientation$ 
Horizontal=�6%   $��0q�6>   .+�54      �� $��5,0,10,0q�6G    d� 	IsChecked 6'   �� *	IsChecked+6G   �� 55      $��Center=�6B   ��6(   �� *
FileName+6B   �� 56      $��10,0,0,0q�6G   $��Center=�6Y   
���������6(   �� *FileExtension+6t   57      58      5:      �� 5<      -   5        #���  5<      $��Исправить��6A   $
��Left=�6U   $��430,260,0,0q�5=      $	��Top=�6&   
���  ��
�������
�������$	��200��5?   $   $��60��60   $��20(�5@      $��
Constantia)�6   67   �� 5B      -   5        #���  5B      $��Выбрать всё��6A   $
��Left=�6W   $��695,117,0,0q�5C      $	��Top=�6"   
���  ��
�������
�������$��85��5E       $��30��6+   $��
Constantia)�67   5F      �� 5H      -	   5        #���  5H      $��Отменить всё��6A   $
��Left=�6X   $��695,152,0,0q�5I      $	��Top=�6"   
���  ��
�������
�������$��85��5K       $��30��6+   $��
Constantia)�67   5L      �� 5N      $��32��6   $��	0,0,120,5q�6    $��Bottom=�63   $��32��6N   $��
/Union.png��6Y   $��Right=�6m   �� 5O      $��Группа ПТК��6   $
��Left=�6)   $��662,386,0,0q�6D   $	��Top=�6Y   $
��Bold"�6q   $��Cambria Math)�6�   ���	#00000000
���  ��6�   �� 5P      $s��mПРОМЫШЛЕННО-ИНЖИНИРИНГОВЫЙ ХОЛДИНГ В ТРАНСПОРТНОЙ ОТРАСЛИ��6   $
��Left=�5Q      $��668,404,1,0q�5R      $	��Top=�5S      $��6(�5T      $
��Wrap=�5U      �� 5V      -
   5        $ ButtonClear��5V      #���  6*   $��Очистить��6V   $
��Left=�6i   $��695,217,0,0q�5W      $	��Top=�6&   
���  ��
�������
�������$��85��5Y   $   $��30��6/   $��
Constantia)�6;   5Z      �� 5[      -   5        $ ButtonExclude��5[      #���  6,   $��Исключить��6X   $
��Left=�6l   $��695,184,0,0q�5\      $	��Top=�6&   
���  ��
�������
�������$��85��5^   $   $��30��6/   $��
Constantia)�6;   5_      5`      5a      5b      