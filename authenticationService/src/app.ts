import 'reflect-metadata';
import compression from 'compression';
import cookieParser from 'cookie-parser';
import express from 'express';
import helmet from 'helmet';
import hpp from 'hpp';
import morgan from 'morgan';
import { useExpressServer } from 'routing-controllers';
import { NODE_ENV, PORT, LOG_FORMAT, ORIGIN, CREDENTIALS, YANDEX_CLIENT_ID, YANDEX_CLIENT_SECRET } from '@config';
import { logger, stream } from '@utils/logger';
import passport from 'passport';
import YandexStrategy from 'passport-yandex';
import session from 'express-session';
import { authenticate, handleSuccessfullAuthentication } from './controllers/auth.controller';

class App {
  public app: express.Application;
  public env: string;
  public port: string | number;
  public user = null;

  constructor() {
    this.app = express();
    this.env = NODE_ENV || 'development';
    this.port = PORT || 3000;

    this.initializePassport();
    this.initializeMiddlewares();
    this.initializeYandexAuth();
    this.initializeRoutes();
  }

  public listen() {
    this.app.listen(this.port, () => {
      logger.info(`=================================`);
      logger.info(`======= ENV: ${this.env} =======`);
      logger.info(`🚀 App listening on the port ${this.port}`);
      logger.info(`=================================`);
    });
  }

  public getServer() {
    return this.app;
  }

  private initializePassport() {
    // const YANDEX_CLIENT_ID = "b9506bafb964487eacd0cab3c472a13c"
    // const YANDEX_CLIENT_SECRET = "5a7ffb7343eb4e3ab7d84d56651a3d31";

    passport.serializeUser(function (user, done) {
      done(null, user);
    });

    passport.deserializeUser(function (obj, done) {
      done(null, obj);
    });

    passport.use(
      new YandexStrategy.Strategy(
        {
          clientID: YANDEX_CLIENT_ID,
          clientSecret: YANDEX_CLIENT_SECRET,
          callbackURL: 'http://localhost:3000/auth/yandex/callback',
        },
        function (accessToken, refreshToken, profile, done) {
          process.nextTick(function () {
            return done(null, profile);
          });
        },
      ),
    );
  }

  private initializeMiddlewares() {
    this.app.use(morgan(LOG_FORMAT, { stream }));
    this.app.use(hpp());
    this.app.use(helmet());
    this.app.use(compression());
    this.app.use(express.json());
    this.app.use(express.urlencoded({ extended: true }));
    this.app.use(cookieParser());
    this.app.use(
      session({
        secret: 'keyboard cat',
        resave: false,
        saveUninitialized: true,
        cookie: { secure: true },
      }),
    );
    this.app.use(passport.initialize());
    this.app.use(passport.session());
  }

  private initializeRoutes() {
    useExpressServer(this.app, {
      cors: {
        origin: ORIGIN,
        credentials: CREDENTIALS,
      },
      defaultErrorHandler: false,
    });
  }

  private initializeYandexAuth() {
    this.app.get(
      '/authenticate/:id',
      (req, res) => {
        passport.authenticate('yandex', { state: req.params.id })(req, res);
      },
      authenticate,
    );

    this.app.get('/auth/yandex/callback', passport.authenticate('yandex', { failureRedirect: '/' }), handleSuccessfullAuthentication);
    this.app.post('/discord/users', (req, res) => {
      console.log(req);
    });
  }
}

export default App;
