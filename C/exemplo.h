#ifndef EXEMPLO_H
#define EXEMPLO_H
#include <stdlib.h>
#include <stdio.h>
#include "list.h"
#include "semaphore.h"
 
int nChilds;
int status;
list_t *list;
int exit1;
pthread_cond_t semexec;
pthread_cond_t semfilhos;
pthread_mutex_t lock;
 int contadorlinhas;
void *monitora (void *arg);
int totaltime;
int contaiteracoes;
int nmr_processes;
 
#endif