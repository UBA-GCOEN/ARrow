import mongoose from "mongoose";


//estimated student schema
const userStudentModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     year: { type: String, required: true },
     branch: { type: String },
     intrest: {type: String},
     roll: { type: Number },
     mobile: { type: Number }

})

export default mongoose.model("userStudents",userStudentModel);