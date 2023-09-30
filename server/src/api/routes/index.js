import express from 'express';
import session from '../middlewares/session.js';
const router = express.Router();
import indexController from "../controllers/index.js";
import authUser from '../middlewares/authUser.js';
import { deleteUser, getDeletePage } from '../controllers/deleteUser.js';
import { logout } from '../middlewares/logout.js';
import { csrfProtect } from '../middlewares/csrfProtection.js';

router.get("/", session, csrfProtect, indexController)
router.get("/logout", session, csrfProtect, logout, indexController)
router.post("/deleteUser", session, authUser, deleteUser)
router.get("/getDeletePage", session, authUser, getDeletePage)

export default router;