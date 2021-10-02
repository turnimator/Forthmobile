
DECIMAL
 5 constant PIN_PWMB
17 constant PIN_IN4
16 constant PIN_IN3
32 constant PIN_IN2
33 constant PIN_IN1
25 constant PIN_PWMA


1 constant LEFT
2 constant RIGHT
3 constant BOTH

1 constant FORWARD
FORWARD negate constant BACKWARD

Forth ledc

: initmotor
	PIN_IN1 output pinmode
	PIN_IN2 output pinmode  
	PIN_IN3 output pinmode
	PIN_IN4 output pinmode  
	PIN_PWMA LEFT ledcAttachPin
	PIN_PWMB RIGHT ledcAttachPin
	LEFT 32767 8 ledcSetup
	RIGHT 32767 8 ledcSetup
	;


: >pwmLeft ( speed -- )
	dup dup 0 > 255 < and IF
		LEFT swap ledcWrite
	THEN
	;
	
: >pwmright ( speed -- )
	dup dup 0 > 255 < and IF
		RIGHT swap ledcWrite
	THEN
	;
	
: >pwmBoth ( speed -- )
	dup . LEFT swap ledcWrite
	dup . RIGHT swap ledcWrite
	;


: motor_stop
	LOW PIN_IN1 pin
	LOW PIN_IN2 pin
	LOW PIN_IN3 pin
	LOW PIN_IN4 pin
	0 0 >pwmBoth
;

: >left_gear ( forward | backward -- )
	forward = if 
		HIGH PIN_IN1 pin
		LOW PIN_IN2 pin
	else
		LOW PIN_IN1 pin
		HIGH PIN_IN2 pin
	then
	;

: >right_gear  ( forward | backward -- )
	forward = if
		HIGH PIN_IN3 pin
		LOW PIN_IN4 pin
	else
		LOW PIN_IN3 pin
		HIGH PIN_IN4 pin
	then
	;

: drive ( leftPwm rightPwm direction -- )
	dup >right_gear >left_gear
	>pwmBoth
	;

: turn ( pwm left|right --)
	left = if 
		forward >left_gear 
		backward >right_gear 
	else 
		backward >left_gear 
		forward >right_gear 
	then 
	dup >pwmBoth
	;

initmotor

