initlaser
initcompass

VARIABLE OBSTACLE_FRONT
VARIABLE OBSTACLE_RIGHT
VARIABLE OBSTACLE_LEFT
VARIABLE course
VARIABLE leftPWM
VARIABLE rightPWM

VARIABLE IS_BOXED_IN

: LookforObstacles
	0 IS_BOXED_IN !
	0 500 servo 200 ms readlaser OBSTACLE_LEFT !
	0 360 servo 200 ms readlaser OBSTACLE_FRONT !
	0 200 servo 200 ms readlaser OBSTACLE_RIGHT !
	0 360 servo 200 ms readlaser OBSTACLE_FRONT !
	;


: addToRightPWM ( value )
	rightPWM @ swap - 
	dup 1 < IF
		0 rightPWM !
	ELSE
		dup 254 > IF
			255 rightPWM !
		ELSE
			rightPWM !
		THEN
	THEN
	;

: addToLeftPWM ( value )
	leftPWM @ swap + 
	dup 1 < IF
		0 rightPWM !
	ELSE
		dup 254 > IF
			255 rightPWM !
		ELSE
			rightPWM !
		THEN
	THEN
	;


: Veer ( amount -- Changes PWM values on the wheels. If amount is negative, veer left )
	2 rshift
	dup dup dup ." Veering: " . cr
	addToRightPWM drop
	addToLeftPWM drop
	leftPWM @ >pwmLeft 
	rightPWM @ >pwmRight
	
	;
	
: CorrectCourse (  )
	readcompass course @ -
	dup ." Course error " . cr
	Veer drop

	;

 : .state
	." FRONT " OBSTACLE_FRONT @ .
	." LEFT " 	OBSTACLE_LEFT @ .
	." RIGHT " 	OBSTACLE_RIGHT @ .	
	cr
	." Course: " course @ . 
	." leftPWM: " leftPWM @ .
	." , rightPWM: " rightPWM @ .
	
	;

: DecideAction
	OBSTACLE_FRONT @ 200 < IF
		." Too close to obstacle, backing up." cr
		150 150 backward drive 1000 ms motor_stop
	THEN
	OBSTACLE_LEFT @ 200 < OBSTACLE_RIGHT @ 200 < and and IF
		1 IS_BOXED_IN ! ." We're boxed in!" cr
		motor_stop
	THEN
	OBSTACLE_LEFT @ OBSTACLE_RIGHT @ < IF 
	." Free sigt to the left."
		course @ 100 - course !
		120 120 forward drive
	ELSE
		." Free sigt to the right."
		course @ 100 + course !
		120 120 forward drive
	THEN
	
	;

: mainloop 
	200 leftPWM !
	200 rightPWM !
	." Starting drive, setting course to "
	leftPWM @ rightPWM @ forward drive
	readcompass dup . course !
	LookForObstacles
	BEGIN
		CorrectCourse
		readlaser dup
		600 < IF
			." Obstacle less that 60cm ahead. Reducing speed." cr
			50 50 >pwmBoth
		THEN
		300 < IF
			motor_stop
			LookForObstacles
			DecideAction
		THEN
		IS_BOXED_IN @ 1 = 
	UNTIL 
	motor_stop
	." End of drive."
	;

