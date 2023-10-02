import express from 'express';
const router = express.Router();
import { authGoogle, callbackGoogle }  from '../controllers/googleAuth.js'
import session from '../middlewares/session.js';

router.get("/auth/google",session,authGoogle)
router.get("/auth/google/callback",session, callbackGoogle)
export default router;