#pragma once

template <typename T>
struct Node {
	int id;
	T value;
	Node<T>* prev;
	Node<T>* next;

	Node(int id, T value, Node<T>* prev, Node<T>* next);
};

template <typename T>
struct List {
	Node<T>* head;
	Node<T>* last;
	int nextID;

	List();
	int Add(T value);
	bool Remove(int id);
	T Get(int id);
};

template <typename T>
Node<T>::Node(int id, T value, Node* prev, Node* next) {
	this->id = id;
	this->value = value;
	this->prev = prev;
	this->next = next;
}

template <typename T>
List<T>::List() {
	this->head = nullptr;
	this->last = nullptr;
	this->nextID = 0;
}

template <typename T>
int List<T>::Add(T value) {
	Node<T>* newNode = new Node<T>(nextID, value, last, nullptr);
	if (last != nullptr) {
		last->next = newNode;
		last = this->last->next;
	}
	else {
		head = newNode;
		last = newNode;
	}

	return nextID++;
}

template <typename T>
bool List<T>::Remove(int id) {
	Node<T>* currNode = head;
	while (currNode != nullptr) {
		if (currNode->id == id) {
			if (currNode->prev != nullptr) {
				currNode->prev->next = currNode->next;
			}
			else {
				head = currNode->next;
			}

			if (currNode->next != nullptr) {
				currNode->next->prev = currNode->prev;
			}
			else {
				last = currNode->prev;
			}
			return true;
		}
		currNode = currNode->next;
	}
	return false;
}

template <typename T>
T List<T>::Get(int id) {
	Node<T>* currNode = head;
	while (currNode != nullptr) {
		if (currNode->id == id) {
			return currNode->value;
		}
		currNode = currNode->next;
	}

	return nullptr;
}