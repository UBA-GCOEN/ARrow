import express from 'express';
import session from '../middlewares/session.js';
const router = express.Router();

import indexController from "../controllers/index.js";
import { logout } from '../middlewares/logout.js';


router.get("/", session, indexController)
router.get("/logout",session , logout, indexController)

export default router;