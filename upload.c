#include<stdio.h>
#include<string.h>	//strlen
#include <unistd.h>
#include <stdlib.h>
#include<sys/socket.h>
#include<arpa/inet.h>	//inet_addr

int main(int argc , char *argv[])
{
	char *dest = "192.168.100.8";
	int socket_desc;
	struct sockaddr_in server;
	char message[2048] , server_reply[2048];
	FILE* f;
	
	if (argc < 2){
		printf("Usage: upload file address\n");
		exit(-1);
	}
	if (argc>2){
		dest = argv[2];
	}
	f = fopen(argv[1], "r");
	//Create socket
	socket_desc = socket(AF_INET , SOCK_STREAM , 0);
	if (socket_desc == -1)
	{
		printf("Could not create socket");
	}
		
	server.sin_addr.s_addr = inet_addr(dest);
	server.sin_family = AF_INET;
	server.sin_port = htons( 23 );

	//Connect to remote server
	if (connect(socket_desc , (struct sockaddr *)&server , sizeof(server)) < 0)
	{
		puts("connect error");
		return 1;
	}
	
	puts("Connected\n");
	
	//Send some data
	while(fgets(message, 255 , f)) {
		message[strlen(message) - 1] = 0;
		strcat(message,"\r\n");
		if( send(socket_desc , message , strlen(message) , 0) < 0)
		{
			puts("Send failed");
			return 1;
		}
		puts(message);
		usleep(50000);
		memset(server_reply, 0 , sizeof(server_reply));
	//Receive a reply from the server
		if( recv(socket_desc, server_reply , 2000 , 0) < 0)	
		{
			puts("recv failed");
		}

		puts(server_reply);
	}
	fclose(f);
	return 0;
}
