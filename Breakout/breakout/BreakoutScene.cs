using ECS;
using ECS.Components;
using ECS.Components.Textures;
using ECS.Resources;
using ECS.Scene;
using ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Breakout.breakout;

class BreakoutGame : Main, IScene
{
    public override IScene StartScene => this;
    
    static Vector2 _paddleSize = new(120, 20);
    static float _gapBetweenPaddleAndFloor = 60f;
    static float _paddleSpeed = 500f;
    // How close can the paddle get to the wall
    static float _paddlePadding = 10f;
    
    // We set the z-value of the ball to 1, so it renders on top in the case of overlapping sprites.
    static Vector3 _ballStartingPosition = new(0f, 150f, 1f);
    //static float _ballDiameter = 30f;
    static Vector2 _ballSize = new(10f, 10f);
    static float _ballSpeed = 400f;
    static Vector2 _initialBallDirection = new(0.5f, -0.5f);
    
    static float _wallThickness = 10f;
    // x coordinates
    static float _leftWall = -450f;
    static float _rightWall = 450f;
    // y coordinates
    static float _bottomWall = 300f;
    static float _topWall = -300f;
    
    static Vector2 _brickSize = new(100f, 30f);
    // These values are exact
    static float _gapBetweenPaddleAndBricks = 270f;
    static float _gapBetweenBricks = 5f;
    // These values are lower bounds, as the number of bricks is computed
    static float _gapBetweenBricksAndCeiling = 20f;
    static float _gapBetweenBricksAndSides = 20f;
    
    static float _scoreboardFontSize = 40f;
    static float _scoreboardTextPadding = 5f;
    
    static Color _backgroundColor = new(0.9f, 0.9f, 0.9f);
    static Color _paddleColor = new(0.3f, 0.3f, 0.7f);
    static Color _ballColor = new(1.0f, 0.5f, 0.5f);
    static Color _brickColor = new(0.5f, 0.5f, 1.0f);
    static Color _wallColor = new(0.8f, 0.8f, 0.8f);
    static Color _textColor = new(0.5f, 0.5f, 1.0f);
    static Color _scoreColor = new(1.0f, 0.5f, 0.5f);
    
    public void Configure(SceneConfig sc)
    {
        sc.AddResource(new ScoreBoard());
        
        sc.AddStartupSystem<Setup>();
        sc.AddUpdateSystem<ApplyVelocity>();
        sc.AddUpdateSystem<MovePaddle>();
        sc.AddUpdateSystem<CheckForCollision>();
        sc.AddUpdateSystem<PlayCollisionSound>();
        sc.AddUpdateSystem<UpdateScoreBoard>();
        sc.AddUpdateSystem<CloseOnEscSystem>();
    }

    class ScoreBoard : IResource
    {
        public int Score { get; set; } = 0;
    }

    record ScoreBoardUi : IComponent, IIdentifier;
    record Paddle : IComponent, IIdentifier;
    record Ball : IComponent, IIdentifier;

    class Velocity(Vector2 speed) : IComponent
    {
        public Vector2 Speed { get; set; } = speed;
    }
    
    record Collider : IComponent;
    record ColliderEvent : IEvent;
    record Brick : IComponent, IIdentifier;
    //record CollisionSound() : IResource; // TODO

    class WallBundle(WallLocation location) : EntityBundle
    {
        public SpriteBundle SpriteBundle { get; } = new()
        {
            Transform = new Transform(location.Location, location.Size),
            Texture = new ColorTexture(_wallColor),
            Extras = [new Collider()],
        };
    }

    class WallLocation
    {
        public static WallLocation Left => new (_leftWall, 0f, true);
        public static WallLocation Right => new (_rightWall, 0f, true);
        public static WallLocation Bottom => new (0f, _bottomWall, false);
        public static WallLocation Top => new (0f, _topWall, false);
        
        public Vector3 Location { get; }
        public Vector2 Size { get; }

        private WallLocation(float x, float y, bool vertical)
        {
            Location = new(x, y, 0f);
            var height = _bottomWall - _topWall; 
            var width = _rightWall - _leftWall;

            Size = vertical
                ? new(_wallThickness, height + _wallThickness)
                : new(width + _wallThickness, _wallThickness);
        }
    }
    
    class Setup : ISystem
    {
        public void Run(ISystemEnv env)
        {
            var window = env.Resource<WindowResource>();
            window.BackgroundColor = _backgroundColor;
            
            var paddleY = _bottomWall - _gapBetweenPaddleAndFloor;
            env.Commander.AddEntity(new SpriteBundle
            {
                Texture = new ColorTexture(_paddleColor),
                Transform = new Transform(new (0f, paddleY, 0f), _paddleSize),
                Extras = [new Paddle(), new Collider()],
            });
            
            env.Commander.AddEntity(new SpriteBundle
            {
                Texture = new ColorTexture(_ballColor),
                Transform = new Transform(_ballStartingPosition, _ballSize),
                Extras = [new Ball(), new Velocity(Vector2.Normalize(_initialBallDirection) * _ballSpeed)],
            });
            
            
            // env.Commander.AddEntity(new TextBundle // TODO
            
            // Walls
            env.Commander.AddEntity(new WallBundle(WallLocation.Left));
            env.Commander.AddEntity(new WallBundle(WallLocation.Right));
            env.Commander.AddEntity(new WallBundle(WallLocation.Bottom));
            env.Commander.AddEntity(new WallBundle(WallLocation.Top));
            
            // Bricks
            var totalWidth = (_rightWall - _leftWall) - 2f * _gapBetweenBricksAndSides;
            var bottomEdge = paddleY - _gapBetweenPaddleAndBricks + _brickSize.Y / 2;
            var totalHeight = bottomEdge + _topWall + _gapBetweenBricksAndCeiling;

            var nColumns = Math.Floor(totalWidth / (_brickSize.X + _gapBetweenBricks));
            var nRows = Math.Floor(Math.Abs(totalHeight) / (_brickSize.Y + _gapBetweenBricks));
            
            var leftEdge = -nColumns / 2 * (_brickSize.X + _gapBetweenBricks) + _brickSize.X / 2;

            for (var row = 0; row < nRows; row++)
            {
                for (var column = 0; column < nColumns; column++)
                {
                    var brickPosition = new Vector3((float) 
                        leftEdge + column * (_brickSize.X + _gapBetweenBricks),
                        bottomEdge - row * (_brickSize.Y + _gapBetweenBricks),
                        0f
                    );
                    env.Commander.AddEntity(new SpriteBundle
                    {
                        Texture = new ColorTexture(_brickColor),
                        Transform = new Transform(brickPosition, _brickSize),
                        Extras = [new Brick(), new Collider()],
                    });
                }
            }
        }
    }
    
    class MovePaddle : ISystem
    {
        public void Run(ISystemEnv env)
        {
            var keyboard = env.Resource<KeyboardResource>();
            var delta = env.Resource<TimeDelta>();
            var paddleTransform = env.Query<Transform>().With<Paddle>().Component();

            var direction = 0f;
            if (keyboard.IsKeyPressed(Keys.Left))
            { 
                direction -= 1f;
            }

            if (keyboard.IsKeyPressed(Keys.Right))
            { 
                direction += 1f;
            }
            
            // Calculate the new horizontal paddle position based on player input
            var newPosition = paddleTransform.Translation.X + direction * _paddleSpeed * delta.Elapsed.TotalSeconds;

            // Update the paddle position,
            // making sure it doesn't cause the paddle to leave the arena
            var leftBound = _leftWall + _wallThickness / 2f + _paddleSize.X / 2f + _paddlePadding;
            var rightBound = _rightWall - _wallThickness / 2f - _paddleSize.X / 2f - _paddlePadding;

            paddleTransform.Translation = paddleTransform.Translation with
            {
                X = float.Clamp((float) newPosition, leftBound, rightBound),
            };
        }
    }
    
    class ApplyVelocity : ISystem
    {
        public void Run(ISystemEnv env)
        {
            var delta = env.Resource<TimeDelta>();
            var deltaSecs = delta.Elapsed.TotalSeconds;
            foreach (var (transform, velocity) in env.Query<Transform, Velocity>().Components())
            {
                transform.Translation = transform.Translation with
                {
                    X = transform.Translation.X + velocity.Speed.X * (float) deltaSecs,
                    Y = transform.Translation.Y + velocity.Speed.Y * (float) deltaSecs,
                };
            }
        }
    }
    
    
    class CheckForCollision : ISystem
    {
        public void Run(ISystemEnv env)
        {
            var (ballVelocity, ballTransform) = env.Query<Velocity, Transform>().With<Ball>().Component();
            var colliderEntities = env.Query<Collider, Transform>().Entities();
            var scoreBoard = env.Resource<ScoreBoard>();

            var bRect = ballTransform.Rect;

            foreach (var entity in colliderEntities)
            {
                var transform = entity.Component<Transform>();
                var isBrick = entity.Component<Brick?>() is not null;
                var rect = transform.Rect;

                if (!bRect.Intersects(rect))
                    continue;

                if (isBrick)
                {
                    env.Commander.RemoveEntity(entity);
                    scoreBoard.Score += 1;
                }

                var right = bRect.Right - rect.Left;
                var left = bRect.Left - rect.Right;
                var top = bRect.Top - rect.Bottom;
                var bottom = bRect.Bottom - rect.Top;

                var vertical = Math.Min(left, right);
                var horizontal = Math.Min(top, bottom);

                if (vertical >= horizontal)
                {
                    ballVelocity.Speed = ballVelocity.Speed with
                    {
                        X = ballVelocity.Speed.X * -1,
                    };
                }
                else
                {
                    ballVelocity.Speed = ballVelocity.Speed with
                    {
                        Y = ballVelocity.Speed.Y * -1,
                    };
                }
            }
        }

    }
    
    class PlayCollisionSound : ISystem
    {
        public void Run(ISystemEnv env)
        {
            // TODO
            // fn play_collision_sound(
            //     mut commands: Commands,
            //     mut collision_events: EventReader<CollisionEvent>,
            //     sound: Res<CollisionSound>,
            // ) {
            //     // Play a sound once per frame if a collision occurred.
            //     if !collision_events.is_empty() {
            //         // This prevents events staying active on the next frame.
            //         collision_events.clear();
            //         commands.spawn(AudioBundle {
            //             source: sound.0.clone(),
            //             // auto-despawn the entity when playback finishes
            //             settings: PlaybackSettings::DESPAWN,
            //         });
            //     }
            // }

        }
    }
    
    class UpdateScoreBoard : ISystem
    {
        public void Run(ISystemEnv env)
        {
            // TODO
            // fn update_scoreboard(scoreboard: Res<Scoreboard>, mut query: Query<&mut Text, With<ScoreboardUi>>) {
            //     let mut text = query.single_mut();
            //     text.sections[1].value = scoreboard.score.to_string();
            // }

        }
    }
}