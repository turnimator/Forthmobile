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
		0 500 servo 150 ms readlaser OBSTACLE_LEFT !
		0 360 servo 150 ms readlaser OBSTACLE_FRONT !
		0 200 servo 150 ms readlaser OBSTACLE_RIGHT !
		0 360 servo 150 ms readlaser OBSTACLE_FRONT !
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
	OBSTACLE_FRONT @ 
	dup 150 <
	IF
		motor_stop ." Obstacle less than 15cm ahead, stopping." cr
	THEN

	dup 100 < OBSTACLE_LEFT @ 100 < OBSTACLE_RIGHT @ 100 < and and 
	IF
		1 IS_BOXED_IN ! ." We're boxed in!" cr
	THEN
	OBSTACLE_LEFT @
	200 < IF 
		50 veer
	THEN
	OBSTACLE_RIGHT @
	200 < IF 
		-50 veer
	THEN
	;

: mainloop 
	200 leftPWM !
	200 rightPWM !
	." Starting drive, setting course to "
	leftPWM @ rightPWM @ forward drive
	readcompass dup . course !
	BEGIN
		CorrectCourse
		LookforObstacles
		DecideAction
		IS_BOXED_IN @ 1 = 
	UNTIL 
	motor_stop
	." End of drive."
	;

