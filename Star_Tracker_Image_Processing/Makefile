CC= g++
CCOPTS= --std=c++11

BINDIR= ./bin

DEPS= imgProc.h

all: $(BINDIR)/libImgProc.dll

$(BINDIR)/libImgProc.dll: $(BINDIR)/imgProc.o
	$(CC) -shared -o $(BINDIR)/libImgProc.dll $(BINDIR)/imgProc.o

$(BINDIR)/imgProc.o: $(DEPS) imgProc.cpp
	$(CC) -c $(CCOPTS) imgProc.cpp -o $(BINDIR)/imgProc.o

.PHONY: clean

clean:
	rm $(BINDIR)/*

deploy:	$(BINDIR)/libImgProc.dll
	cp -f $(BINDIR)/libImgProc.dll ../PointGrey_Cam_Acq/obj/x64/Debug/