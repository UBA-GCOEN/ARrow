import express from 'express';
import session from '../middlewares/session.js'
import { csrfProtect } from '../middlewares/csrfProtection.js';
import authUser from '../middlewares/authUser.js'
import { deleteEvent, 
    getSpecificEvent, 
    getAllEvent, 
    updateEvent,
    createEvent 
} from '../controllers/events.js';


const router = express.Router();

router.post("/createEvent", session, csrfProtect, authUser, createEvent)
router.put("/updateEvent", session, csrfProtect, authUser, updateEvent)
router.get("/getAllEvent", getAllEvent)
router.get("/getSpecificEvent", getSpecificEvent)
router.delete("/deleteEvent", deleteEvent)

export default router;