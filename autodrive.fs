initlaser
initcompass

0 value course

0 value LAST_OBSTACLE

0 value OBSTACLE_FRONT
0 value OBSTACLE_RIGHT
0 value OBSTACLE_LEFT

0 value leftPWM
0 value rightPWM

0 value IS_BOXED_IN

: plotRadar
	cr
		\ Estimate how far we traveled
	OBSTACLE_FRONT LAST_OBSTACLE - ." @m " . cr
	." @p 0 " OBSTACLE_FRONT . cr 
	." @p -45 " OBSTACLE_LEFT . cr 
	." @p 45 " OBSTACLE_RIGHT . cr 
	." @h " COURSE . cr 
	;
	
: ReadSensors
	0 to IS_BOXED_IN
	OBSTACLE_FRONT to LAST_OBSTACLE \ Save old
	readlaser1 to OBSTACLE_FRONT
	readlaser3 to OBSTACLE_LEFT 
	readlaser2 to OBSTACLE_RIGHT
\	readcompass to course
	;

: addToRightPWM ( value )
	rightPWM swap - 
	dup 1 < IF
		0 to rightPWM
	ELSE
		dup 254 > IF
			255 to rightPWM
		ELSE
			to rightPWM
		THEN
	THEN
	;

: addToLeftPWM ( value )
	leftPWM swap + 
	dup 1 < IF
		0 to rightPWM
	ELSE
		dup 254 > IF
			255 to rightPWM
		ELSE
			to rightPWM
		THEN
	THEN
	;


: Veer ( amount -- Changes PWM values on the wheels. If amount is negative, veer left )
	2 rshift
	dup dup dup ." Veering: " . cr
	addToRightPWM drop
	addToLeftPWM drop
	leftPWM >pwmLeft 
	rightPWM >pwmRight
	
	;
	
: CorrectCourse (  )
	readcompass course -
	dup ." Course error " . cr
	Veer drop

	;

 : .state
	." FRONT " OBSTACLE_FRONT .
	." LEFT " 	OBSTACLE_LEFT .
	." RIGHT " 	OBSTACLE_RIGHT .	
	cr
	." Course: " course . 
	." leftPWM: " leftPWM .
	." , rightPWM: " rightPWM .
	
	;

: DecideAction
	OBSTACLE_FRONT 200 < IF
		." Too close to obstacle, backing up." cr
		150 150 backward drive 1000 ms motor_stop
		EXIT
	THEN
	OBSTACLE_LEFT 200 < OBSTACLE_RIGHT 200 < and and IF
		1 to IS_BOXED_IN ." We're boxed in!" cr
		motor_stop
		EXIT
	THEN
	OBSTACLE_LEFT 100 < IF
		50 right turn 1000 ms
		EXIT
	THEN
		OBSTACLE_RIGHT 100 < IF
		50 left turn 1000 ms
		EXIT
	THEN

	OBSTACLE_LEFT 200 < IF
		200 50 forward drive 1000 ms
		EXIT
	THEN
		OBSTACLE_RIGHT 200 < IF
		50 200 forward drive 1000 ms
		EXIT
	THEN

	180 180 forward drive
	;

: mainloop 
	readSensors
	200 to leftPWM 
	200 to rightPWM 
	." Starting drive, setting course to "
	leftPWM rightPWM forward drive
	BEGIN
		plotRadar
		readSensors
		CorrectCourse
		DecideAction
		IS_BOXED_IN 1 = 
	UNTIL 
	motor_stop
	." End of drive."
	;



