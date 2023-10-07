import express from 'express';
import session from '../middlewares/session.js'
import { csrfProtect } from '../middlewares/csrfProtection.js';
import authUser from '../middlewares/authUser.js'
import { deleteNotification, getNotification, sendNotification } from '../controllers/notification.js';

const router = express.Router();

router.post("/send", session, authUser, sendNotification)
router.get("/get", session, authUser, getNotification)
router.delete("/delete", session, authUser, deleteNotification)
export default router;