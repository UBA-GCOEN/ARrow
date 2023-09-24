import express from 'express';
import session from '../middlewares/session.js';
const router = express.Router();
import indexController from "../controllers/index.js";
import authUser from '../middlewares/authUser.js';
import { deleteUser, getDeletePage } from '../controllers/deleteUser.js';
import { logout } from '../middlewares/logout.js';
import { csrfProtect } from '../middlewares/csrfProtection.js';
import { sendResetEmail, verifyEmail, updatePassword } from '../controllers/forgotPassword.js';
import { changePassword } from '../controllers/changePassword.js';



router.get("/", session, csrfProtect, indexController)
router.get("/logout", session, csrfProtect, logout, indexController)
router.post("/deleteUser", session, csrfProtect, authUser, deleteUser)
router.get("/getDeletePage", session, csrfProtect, authUser, getDeletePage)
router.post("/sendEmail", session, csrfProtect, authUser, sendResetEmail)
router.get("/verifyEmail", verifyEmail)
router.post("/updatePassword", session, csrfProtect, authUser, updatePassword)
router.post("/changePassword", session, csrfProtect, authUser, changePassword)

export default router;