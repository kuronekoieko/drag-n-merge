public enum ScreenState
{
    INITIALIZE,
    START,
    GAME,
    RESULT,
}

public enum GameState
{
    IN_PROGRESS_TIMER,
    RESET_TIMER,
    MOVE_UP_ANIM,
}

public enum BlockState
{
    STOP,
    FALL,
    DRAG,
}

public enum BlockType
{
    NUMBER,
    JOKER,
    ALL_UP,
    FALL_LINE,
}