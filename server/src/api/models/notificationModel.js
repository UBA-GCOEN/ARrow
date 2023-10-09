import mongoose, { Schema } from "mongoose";


const notificationModel = new mongoose.Schema({
    _id: { type: Schema.Types.ObjectId, default: () => new mongoose.Types.ObjectId(), required:true },
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