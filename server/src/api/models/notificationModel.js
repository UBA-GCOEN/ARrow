import mongoose from "mongoose";


const notificationModel = new mongoose.Schema({
    title: { type: String, required: true },
    message: { type:String, required:true },
    senderEmail: { type:String, required:true },
    senderName: { type:String, required:true },
    senderRole: { type:String, required:true },
    receiverRole: [{
        type: String,
        required: true,
    }],
    receiverBranch: [{ type: String }],
    receiverYear: [{ type: Number }],
    createdAt: { type: Date, default: Date.now }
})

export default mongoose.model("notifications", notificationModel);