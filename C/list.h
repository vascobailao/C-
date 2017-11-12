/*
 * list.h - definitions and declarations of the integer list 
 */
 
#ifndef LIST_H
 #define LIST_H
#include <stdlib.h>
#include <stdio.h>
#include <time.h>
 
 FILE *fptr;
 
/* lst_iitem - each element of the list points to the next element */
typedef struct lst_iitem {
   int pid;
   time_t starttime;
   time_t endtime; 
   struct lst_iitem *next;
   int iterador;
} lst_iitem_t;
 
/* list_t */
typedef struct {
   lst_iitem_t * first;
} list_t;
 
 
 
/* lst_new - allocates memory for list_t and initializes it */
list_t* lst_new();
 
/* lst_destroy - free memory of list_t and all its items */
void lst_destroy(list_t *);
 
/* insert_new_process - insert a new item with process id and its start time in list 'list' */
void insert_new_process(list_t *list, int pid, time_t starttime);
 
/* lst_remove - remove first item of value 'value' from list 'list' */
void update_terminated_process(list_t *list, int pid, time_t endtime,int* iterador, int* totaltime);


 
/* lst_print - print the content of list 'list' to standard output */
//void lst_print(list_t *list, int iterador, int totaltime);
 
#endif