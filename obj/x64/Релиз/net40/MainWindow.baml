   M S B A M L   `   `   `  ����   STEP-corrector    STEP_corrector.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     U QWpfAnimatedGif, Version=2.0.0.0, Culture=neutral, PublicKeyToken=9e7cd3b544a090dc,gif"http://wpfanimatedgif.codeplex.com  -     �Name$  STEP_corrector�� � Title$ STEP-corrector��$	��450��$	��800�� � AllowsTransparency .  � Icon$ /faviconV2.png�� � 
ResizeMode$ NoResize=� � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=�����  �� ImageSource$ /background.png��.��� -    �Name$ MainGrid��	 � RowDefinitions	 �� $��25 ��� $��* �.+�� $��0�����#2F3136$��46,0,0,424.6q�$��2��.+��� -   
 ��Name$
 
TitelLabel��$��STEP-corrector��$
��Left=�$	��Top=�
��������� -    ��Name$ 
ButtonQuit��   CloseButtonStyle1#���   $��X��$
��Left=�$��28��$��0,0,0,0q�}�.  J� SnapsToDevicePixels . $��1��
�������$��20(�$��Corbel)�$��25��$��	725,0,0,0q�$	��Top=��� -   $ ButtonCollaps��#���   $��__��$
��Left=�$��29��$��0,0,0,0q�}�.  . $��1��
�������$��20(�$��Corbel)�$��25��$��	695,0,0,0q�$	��Top=��� -   $ ButtonAutocorrecrList��  ListButtonStyle#���  $�� ��$
��Left=�$��30��$��0,0,0,0q�}�.  . $��1��
�������$��20(�$��Corbel)�$��25��$��	664,0,0,0q�$	��Top=�
������� J� RenderTransformOrigin$ 0.5,0.5&�z�V� .~��� �� �� T� ���� $* $pack://siteoforigin:,,,/listIcon.png��� $��1�� � ColumnDefinitions �� $	��39* ��� .+��� -     ImportButtonStyle#���  $��Загрузить STEP��$
��Left=�$��120,261,0,0q�$	��Top=�$	��200��$��60��$��20(�$��
Constantia)��� $
��Left=�$��
143,87,0,0q�$	��Top=�$	��150��$��/folderIcon.png���� 
���  ��
���  ��$��3��$	��300��$��402,62,375,62.8q��� $)��#Исправить STEP-файлы:��$
��Left=�$��
434,81,0,0q�$	��Top=�$��18(�$��
Constantia)� � ToolTip$Y SОтметьте файлы, которые необходимо исправить���� -    ��Name$ FilesListBox��$	��130��
�������$��1q�$
��Left=�$	��260��$��440,118,0,176.8q� �� SelectionMode$ Single=����� .R���  �� Orientation$ 
Horizontal=�$��0q�.+��� $��5,0,10,0q� d� 	IsChecked �� *	IsChecked+�� $��Center=����� *
FileName+�� $��10,0,0,0q�$��Center=�
����������� *FileExtension+�� -	   #���  $��Исправить��$
��Left=�$��440,261,0,0q�$	��Top=�
�������$	��200��$��60��$��20(�$��
Constantia)��� -
   #���  $��Выбрать всё��$
��Left=�$��705,118,0,0q�$	��Top=�
�������$��85��$��30��$��
Constantia)�$��2���� -   #���  $��Отменить всё��$
��Left=�$��705,153,0,0q�$	��Top=�
�������$��85��$��30��$��
Constantia)�$��2���� $��32��$��	0,0,120,5q�$��Bottom=�$��32��$��
/Union.png��$��Right=��� $��Группа ПТК��$
��Left=�$��662,386,0,0q�$	��Top=�$
��Bold"�$��Cambria Math)����	#00000000
���  ���� $s��mПРОМЫШЛЕННО-ИНЖИНИРИНГОВЫЙ ХОЛДИНГ В ТРАНСПОРТНОЙ ОТРАСЛИ��$
��Left=�$��668,404,1,0q�$	��Top=�$��6(�$
��Wrap=��� -   $ ButtonClear��#���  $��Очистить��$
��Left=�$��705,218,0,0q�$	��Top=�
�������$��85��$��30��$��
Constantia)�$��2���� -   $ ButtonExclude��#���  $��Исключить��$
��Left=�$��705,185,0,0q�$	��Top=�
�������$��85��$��30��$��
Constantia)�$��2���� ���#2F3136$
��Left=�$	��450��$��
0,0,0,-0.4q�$	��Top=�$��41��$��2���� $
��STEP��$
��Left=�$��
2,45.2,0,0q�$��1��$	��Top=�$��37���� $
��Left=�$��21��$��9,2,0,0q�$	��Top=�$��21��$��	Union.png���� ���	#FF515868$
��Left=�$��3��$��	1,2.2,0,0q�$��1��$	��Top=�$��39��