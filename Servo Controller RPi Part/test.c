#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <unistd.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include <regex.h>

int process_input(char* s)
{
	// Execute command to change PWM duty cycle.
	// Expects 's' to be a number, will fail otherwise. 
	char cmd[25];
	strcpy(cmd, "gpio -g pwm 18 ");
	strcat(cmd, s);
	system(cmd);

	// Will return 0 on success.
	return 0;
}

int main(int argc, char **argv)
{
	// Check if the arg count is right.
	// Expecting a port number (sudo ./test <port_no>
	if (argc < 2)
	{
		fprintf(stderr, "ERROR, no port provided.\n");
		exit(1);
	}

	// Convert the port number to an integer.
	int portno = atoi(argv[1]);
	printf("Port number %d\n", portno);

	// Declare some variables which will be re-used.
	int sockfd, newsockfd, clilen;
	char buffer[256];
	struct sockaddr_in serv_addr, cli_addr;
	int n;

	// Create a new socket.
	printf("Creating socket...");
	sockfd = socket(AF_INET, SOCK_STREAM, 0);
	printf("done.\n");
	if (sockfd < 0)
	{
		perror("Error opening socket.");
		exit(1);
	}
	
	// Clear the serv_addr struct data.
	bzero((char *) &serv_addr, sizeof(serv_addr));

	// Set up the serv_addr struct.
	serv_addr.sin_family = AF_INET;
	serv_addr.sin_addr.s_addr = INADDR_ANY;
	serv_addr.sin_port = htons(portno); // Make sure this is in network byte order.

	// Attempt to bind socket.
	printf("Attempting to bind socket...");
	if (bind(sockfd, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0)
	{
		perror("Error on binding.");
		exit(1);
	}
	printf("done.\n");

	// Mark as listener for client communication.
	printf("Marking socket as listener...");	
	listen(sockfd, 5);
	printf("done.\n");
	
	// Start socket connection.
	// Loop to re-listen for connection if connection ends.
	while (1)
	{

		// Accept client connection.
		printf("Accepting client connection...");
		clilen = sizeof(cli_addr);
		newsockfd = accept(sockfd, (struct sockaddr *) &cli_addr, &clilen);
		if (newsockfd < 0)
		{
			perror("Error on accept.");
			exit(1);
		}
		printf("accepted.\n");

		// Loop while the connection is active.
		while (1)
		{
			// Read from socket.
			bzero(buffer, 256);
			n = recv(newsockfd, buffer, 256, 0);
			if (n < 0)
			{
				perror("Error reading from socket.");
				exit(1);
			}

			// If byte-count equals 0, probably FIN packet was received.
			if (n == 0)
			{
				// Closing connection.
				printf("FIN packet received, closing connection.\n");
			
				// Break from the loop.	
				break;
			}
		
			// Otherwise, process the input.	
			printf("Received input: %s\n", buffer);
			process_input(buffer);
		}
	}

	return 0;
}

