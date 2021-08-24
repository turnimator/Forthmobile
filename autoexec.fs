
DECIMAL
22 constant PIN_SCL
21 constant PIN_SDA
18 constant PIN_ECHO
19 constant PIN_TRIG
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

( Ultrasound SR04 high level forth library Atle Bergstrøm  2021  ) 
( ... delay between lines is useful when uploading over WIFI )
: init 
	PIN_TRIG output pinmode 
	PIN_ECHO input pinmode 
	PIN_IN1 output pinmode
	PIN_IN2 output pinmode  
	PIN_IN3 output pinmode
	PIN_IN4 output pinmode  
	PIN_PWMA LEFT ledcAttachPin
	PIN_PWMB RIGHT ledcAttachPin
	LEFT 4096 8 ledcSetup
	RIGHT 4096 8 ledcSetup
	;

init

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

: trig low PIN_TRIG pin 2 ms high PIN_TRIG pin 10 ms low PIN_TRIG pin ;
: echo PIN_ECHO HIGH 30000 pulsein ;
: scan trig echo . cr ;



z" GET_E93782" z" 69940105" login telnetd server

