   M S B A M L   `   `   `  ����   STEP-corrector    STEP_corrector.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     U QWpfAnimatedGif, Version=2.0.0.0, Culture=neutral, PublicKeyToken=9e7cd3b544a090dc,gif"http://wpfanimatedgif.codeplex.com  -     �Name$  STEP_corrector�� � Title$ STEP-corrector��$	��450��$	��800�� � Icon$ /faviconV2.png�� � 
ResizeMode$ NoResize=� � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=�����  �� ImageSource$ /background.png��.��� -    �Name$ MainGrid�� � RowDefinitions �� $��25 ��� $��* �.+�� $��0�����	#FF121212.+��� $
��Left=�$��Center=�$��14��$��14��$��/faviconV2.png��$��1��$��1��$��10,0,0,0q�}�. 	 J� SnapsToDevicePixels	 . �� -   
 ��Name$
 
TitelLabel��$��STEP-corrector��$
��Left=�$	��Top=�
�������$��25,0,0,0q��� -    ��Name$ 
ButtonQuit��   CloseButtonStyle1#���   $��X��$
��Left=�$��30��$��0,0,0,0q�}�. 	 . $��1��
����������	#00000000$��20(�$��Corbel)����	#00DDDDDD$��25��$��	770,0,0,0q�$	��Top=�� $��1�� � ColumnDefinitions �� $	��39* ��� .+��� -     ImportButtonStyle#���  $��Загрузить STEP��$
��Left=�$��
80,260,0,0q�$	��Top=�
���  ��
�������
�������$	��200��$��60��$��20(�$��
Constantia)��� $
��Left=�$��
105,87,0,0q�$	��Top=�$	��150��$��/folderIcon.png���� 
���  ��
���  ��$��3��$	��300���� $)��#Исправить STEP-файлы:��$
��Left=�$��
424,80,0,0q�$	��Top=�$��18(�$��
Constantia)� � ToolTip$Y SОтметьте файлы, которые необходимо исправить���� -    ��Name$ FilesListBox��$	��130��
�������$��1q�$
��Left=�$	��260��$��430,117,0,178q� �� SelectionMode$ Single=����� .R���  �� Orientation$ 
Horizontal=�$��0q�.+��� $��5,0,10,0q� d� 	IsChecked �� *	IsChecked+�� $��Center=����� *
FileName+�� $��10,0,0,0q�$��Center=�
����������� *FileExtension+�� -   #���  $��Исправить��$
��Left=�$��430,260,0,0q�$	��Top=�
���  ��
�������
�������$	��200��$��60��$��20(�$��
Constantia)��� -   #���  $��Выбрать всё��$
��Left=�$��695,117,0,0q�$	��Top=�
���  ��
�������
�������$��85��$��30��$��
Constantia)��� -	   #���  $��Отменить всё��$
��Left=�$��695,152,0,0q�$	��Top=�
���  ��
�������
�������$��85��$��30��$��
Constantia)��� $��32��$��	0,0,120,5q�$��Bottom=�$��32��$��
/Union.png��$��Right=��� $��Группа ПТК��$
��Left=�$��662,386,0,0q�$	��Top=�$
��Bold"�$��Cambria Math)����	#00000000
���  ���� $s��mПРОМЫШЛЕННО-ИНЖИНИРИНГОВЫЙ ХОЛДИНГ В ТРАНСПОРТНОЙ ОТРАСЛИ��$
��Left=�$��668,404,1,0q�$	��Top=�$��6(�$
��Wrap=��� -
   $ ButtonClear��#���  $��Очистить��$
��Left=�$��695,217,0,0q�$	��Top=�
���  ��
�������
�������$��85��$��30��$��
Constantia)��� -   $ ButtonExclude��#���  $��Исключить��$
��Left=�$��695,184,0,0q�$	��Top=�
���  ��
�������
�������$��85��$��30��$��
Constantia)�