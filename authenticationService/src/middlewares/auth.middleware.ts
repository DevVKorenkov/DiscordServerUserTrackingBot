import { NextFunction, Response } from 'express';
import { verify } from 'jsonwebtoken';
import { SECRET_KEY } from '@config';
import { HttpException } from '@exceptions/HttpException';
import { DataStoredInToken, RequestWithUser } from '@interfaces/auth.interface';
// import userModel from '@models/users.model';
import passport from 'passport';

const authMiddleware = (strategy: string) =>  async (req: RequestWithUser, res: Response, next: NextFunction) => await passport.authenticate(strategy)


export default authMiddleware;
