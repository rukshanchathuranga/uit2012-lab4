Imports Microsoft.Kinect

'------------------------------------------------------------------------------
' <copyright file="KinectAudioViewer.xaml.cs" company="Microsoft">
'     Copyright (c) Microsoft Corporation.  All rights reserved.
' </copyright>
'------------------------------------------------------------------------------

Namespace Microsoft.Samples.Kinect.WpfViewers

	''' <summary>
	''' Interaction logic for KinectAudioViewer.xaml
	''' </summary>
	Partial Public Class KinectAudioViewer
		Inherits ImageViewer
		Private angle As Double
		Private soundSourceAngle As Double

		Public Sub New()
			InitializeComponent()
			Me.MarkWidth = 0.05
			Me.SoundSourceWidth = 0.05
			Me.BeamAngleInDegrees = 0
			Me.BeamDisplayText = Nothing
			Me.SoundSourceAngleInDegrees = 0
			Me.SoundSourceDisplayText = Nothing
		End Sub

		''' <summary>
		''' Gets or sets string overlayed on beam indicator
		''' </summary>
		Public Property BeamDisplayText() As String
			Get
				Return txtDisplayBeam.Text
			End Get

			Set(ByVal value As String)
				txtDisplayBeam.Text = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets string overlayed on sound source indicator
		''' </summary>
		Public Property SoundSourceDisplayText() As String
			Get
				Return txtDisplaySource.Text
			End Get

			Set(ByVal value As String)
				txtDisplaySource.Text = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets width of the beam mark, in the 0-0.5 range
		''' </summary>
		Public Property MarkWidth() As Double

		''' <summary>
		''' Gets or sets audio beam angle, in degrees
		''' </summary>
		Public Property BeamAngleInDegrees() As Double
			Get
				Return Me.angle
			End Get

			Set(ByVal value As Double)
				' save RAW sensor value
				Me.angle = value

				' Angle is in Degrees, so map the MinBeamAngle..MaxBeamAngle range to 0..1
				' and clamp
				Dim gradientOffset As Double = (value / (KinectAudioSource.MaxBeamAngle - KinectAudioSource.MinBeamAngle)) + 0.5
				If gradientOffset > 1.0 Then
					gradientOffset = 1.0
				End If

				If gradientOffset < 0.0 Then
					gradientOffset = 0.0
				End If

				' Move the gradient stops together
				Me.gsPre.Offset = Math.Max(gradientOffset - Me.MarkWidth, 0)
				gsIt.Offset = gradientOffset
				Me.gsPos.Offset = Math.Min(gradientOffset + Me.MarkWidth, 1)
			End Set
		End Property

		''' <summary>
		''' Gets or sets width of the sound source mark, in the 0-0.5 range
		''' </summary>
		Public Property SoundSourceWidth() As Double

		''' <summary>
		''' Gets or sets sound direction angle, in degrees
		''' </summary>
		Public Property SoundSourceAngleInDegrees() As Double
			Get
				Return Me.soundSourceAngle
			End Get

			Set(ByVal value As Double)
				' save RAW sensor value
				Me.soundSourceAngle = value

				' Angle is in Degrees, so map the MinSoundSourceAngle..MaxSoundSourceAngle range to 0..1
				' and clamp
				Dim gradientOffset As Double = (value / (KinectAudioSource.MaxSoundSourceAngle - KinectAudioSource.MinSoundSourceAngle)) + 0.5
				If gradientOffset > 1.0 Then
					gradientOffset = 1.0
				End If

				If gradientOffset < 0.0 Then
					gradientOffset = 0.0
				End If

				' Move the gradient stops together
				Me.gsPreS.Offset = Math.Max(gradientOffset - Me.SoundSourceWidth, 0)
				gsItS.Offset = gradientOffset
				Me.gsPosS.Offset = Math.Min(gradientOffset + Me.SoundSourceWidth, 1)
			End Set
		End Property

		Protected Overrides Sub OnKinectChanged(ByVal oldKinectSensor As KinectSensor, ByVal newKinectSensor As KinectSensor)
			If oldKinectSensor IsNot Nothing AndAlso oldKinectSensor.AudioSource IsNot Nothing Then
				' remove old handlers
				RemoveHandler oldKinectSensor.AudioSource.BeamAngleChanged, AddressOf AudioSourceBeamChanged
				RemoveHandler oldKinectSensor.AudioSource.SoundSourceAngleChanged, AddressOf AudioSourceSoundSourceAngleChanged
			End If

			If newKinectSensor IsNot Nothing AndAlso newKinectSensor.AudioSource IsNot Nothing Then
				' add new handlers
				AddHandler newKinectSensor.AudioSource.BeamAngleChanged, AddressOf AudioSourceBeamChanged
				AddHandler newKinectSensor.AudioSource.SoundSourceAngleChanged, AddressOf AudioSourceSoundSourceAngleChanged
			End If
		End Sub

		Private Sub AudioSourceSoundSourceAngleChanged(ByVal sender As Object, ByVal e As SoundSourceAngleChangedEventArgs)
			' Set width of mark based on confidence
			Me.SoundSourceWidth = Math.Max(((1 - e.ConfidenceLevel) / 2), 0.02)

			' Move indicator
			Me.SoundSourceAngleInDegrees = e.Angle

			' Update text
			Me.SoundSourceDisplayText = " Sound source angle = " & Me.SoundSourceAngleInDegrees.ToString("0.00") & " deg  Confidence level=" & e.ConfidenceLevel.ToString("0.00")
		End Sub

		Private Sub AudioSourceBeamChanged(ByVal sender As Object, ByVal e As BeamAngleChangedEventArgs)
			' Move our indicator
			Me.BeamAngleInDegrees = e.Angle

			' Update Text
			Me.BeamDisplayText = " Audio beam angle = " & Me.BeamAngleInDegrees.ToString("0.00") & " deg"
		End Sub
	End Class
End Namespace
