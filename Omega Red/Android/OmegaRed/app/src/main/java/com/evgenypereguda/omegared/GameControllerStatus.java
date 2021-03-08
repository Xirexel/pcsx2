package com.evgenypereguda.omegared;

@FunctionalInterface
public interface GameControllerStatus {
    void run(GameController.StatusEnum a_Status);
}
